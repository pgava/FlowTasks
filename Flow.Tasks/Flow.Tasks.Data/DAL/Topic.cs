using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Flow.Tasks.Data.Core;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Data.Core.Interfaces;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;
using Flow.Tasks.Data.Infrastructure;
using Flow.Users.Proxy;
using Flow.Library;

namespace Flow.Tasks.Data.DAL
{
    /// <summary>
    /// Topic
    /// </summary>
    public sealed class Topic : ITopic
    {
        /// <summary>
        /// FlowUsers Service
        /// </summary>
        private readonly IFlowUsersService _usersService;

        public Topic() { }

        public Topic(IFlowUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Create Topic
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="attachments">Attachments</param>
        public int CreateTopic(string title, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments)
        {
            // if to is not empty then this is a direct message
            // otherwise get the to users from followers
            if (string.IsNullOrWhiteSpace(to))
            {
                to = GetBroadcastUsers(from);
            }

            var users = to + ";" + from; // topic visible to sender (from) as well
            var usersToCopy = ParseUsers.GetListUsersName(_usersService, users);

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var topic = new Core.Topic { Title = title, LastChanged = DateTime.Now};
                uofw.Topics.Insert(topic);

                CreateMessage(uofw, topic, true, message, from, to, attachments, usersToCopy);

                uofw.Commit();

                return topic.TopicId;
            }
        }

        /// <summary>
        /// Create Reply
        /// </summary>
        /// <param name="topicId">TopicId</param>
        /// <param name="message">Message</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="attachments">Attachments</param>
        public void CreateReply(int topicId, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var topic = uofw.Topics.First(t => t.TopicId == topicId);
                var topicMessage = uofw.TopicMessages.First(m => m.TopicId == topicId && m.IsTopic);

                if (!string.IsNullOrWhiteSpace(to)) to += ";";
                else to = string.Empty;

                if (!to.Contains(topicMessage.To))
                {
                    to += topicMessage.To;
                }
                if (!to.Contains(topicMessage.From))
                {
                    to += ";" + topicMessage.From;
                }

                var usersToCopy = ParseUsers.GetListUsersName(_usersService, to);

                CreateMessage(uofw, topic, false, message, from, to, attachments, usersToCopy);
                topic.LastChanged = DateTime.Now;

                uofw.Commit();
            }
        }

        /// <summary>
        /// Get Topics For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="topicId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="withReplies"></param>
        /// <returns>List of topics</returns>
        public IEnumerable<TopicInfo> GetTopicsForUser(string user, int topicId, DateTime start, DateTime end,
                                                       string title, string status, int pageIndex, int pageSize, bool withReplies)
        {
            var topicInfo = new List<TopicInfo>();

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var userTopics = TopicsForUser(uofw, user, topicId, title, string.Empty, false);
                
                var skip = pageIndex * pageSize;
                var topicsinPage = userTopics.OrderByDescending(tu => tu.TopicMessage.Topic.LastChanged).Skip(skip).Take(pageSize).ToList();

                var statusNewStr = TopicStatusType.New.ToString();
                var statusReadStr = TopicStatusType.Read.ToString();
                var statusRead = uofw.TopicStatuses.First(s => s.Status == statusReadStr);

                foreach (var tu in topicsinPage)
                {
                    var ti = CreateTopicInfo(tu, uofw, 20, end.AddDays(-1), end, status, withReplies);
                    topicInfo.Add(ti);

                    if (tu.TopicStatus.Status == statusNewStr)
                    {
                        tu.TopicStatus = statusRead;
                    }
                }

                uofw.Commit();
            }

            return topicInfo;
        }

        /// <summary>
        /// Get Replies For User
        /// </summary>
        /// <param name="topicId">TopicId</param>
        /// <param name="user">User</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="showType"></param>
        /// <param name="hasOldReplies"></param>
        /// <returns>List of TopicMessageInfo</returns>
        public IEnumerable<TopicMessageInfo> GetRepliesForUser(int topicId, string user, DateTime start, DateTime end, RepliesShowType showType, out bool hasOldReplies)
        {
            const int recentReplies = 20;

            var replies = new List<TopicMessageInfo>();

            using (var uofw = new FlowTasksUnitOfWork())
            {
                //var userTopics = uofw.TopicUsers.Find(tu => !tu.TopicMessage.IsTopic && tu.TopicMessage.TopicId == topicId && tu.User == user
                //    && tu.TopicMessage.When >= start && tu.TopicMessage.When <= end, tu => tu.TopicMessage, tu => tu.TopicMessage.Topic, tu => tu.TopicStatus)
                //    .OrderByDescending(tu => tu.TopicMessageId);

                var userTopics = uofw.TopicUsers.Find(tu => !tu.TopicMessage.IsTopic && tu.TopicMessage.TopicId == topicId && tu.User == user, tu => tu.TopicMessage.Topic, tu => tu.TopicStatus)
                    .OrderBy(tu => tu.TopicMessageId).ToList();

                var statusNewStr = TopicStatusType.New.ToString();
                var statusReadStr = TopicStatusType.Read.ToString();
                var statusRead = uofw.TopicStatuses.First(s => s.Status == statusReadStr);

                var countReplies = 0;
                var users = new Dictionary<string, string>();
                foreach (var r in userTopics)
                {
                    if (showType == RepliesShowType.All || (showType == RepliesShowType.Recent && countReplies < recentReplies) || 
                        (showType == RepliesShowType.Old && countReplies >= recentReplies))
                    {
                        replies.Add(CreateMessageInfo(r, uofw, users));
                    }
                    if (r.TopicStatus.Status == statusNewStr)
                    {
                        r.TopicStatus = statusRead;
                    }
                    countReplies += 1;
                }
                uofw.Commit();

                hasOldReplies = countReplies > recentReplies;
            }

            return replies;
        }

        /// <summary>
        /// Get Topic Count
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Number of topic unread</returns>
        public int GetTopicCount(string user)
        {
            var statusNewStr = TopicStatusType.New.ToString();

            using (var uofw = new FlowTasksUnitOfWork())
            {
                var userTopics = TopicsForUser(uofw, user, 0, string.Empty, statusNewStr, true);
                return userTopics.Count();
            }
        }

        /// <summary>
        /// Search For Topics
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="pattern">Pattern</param>
        /// <returns>List of topics that match pattern</returns>
        public IEnumerable<SearchInfo> SearchForTopics(string user, string pattern)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                return uofw.TopicUsers.ExecuteStoreQuery<SearchInfo>("select [TopicId], [Message], [Rank] from SearchTopicForUser(@p0, @p1)",
                    user, ConvertPatternFullTextSearch(pattern)).OrderByDescending(t => t.Rank).ToList();
            }
        }


        /// <summary>
        /// Create Message
        /// </summary>
        /// <param name="uofw">FlowTasksUnitOfWork</param>
        /// <param name="topic">Topic</param>
        /// <param name="isTopic">Is Topic</param>
        /// <param name="message">Message</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="attachments">Attachments</param>
        /// <param name="usersToCopy">Users To Copy</param>
        private static void CreateMessage(FlowTasksUnitOfWork uofw, Core.Topic topic, bool isTopic, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments, IEnumerable<string> usersToCopy)
        {
            var newMessage = new TopicMessage { Message = message, IsTopic = isTopic, From = from, To = to, When = DateTime.Now, Topic = topic };
            uofw.TopicMessages.Insert(newMessage);

            var statusNewStr = TopicStatusType.New.ToString();
            var statusReadStr = TopicStatusType.Read.ToString();
            var statusNew = uofw.TopicStatuses.First(s => s.Status == statusNewStr);
            var statusRead = uofw.TopicStatuses.First(s => s.Status == statusReadStr);
            foreach (var u in usersToCopy)
            {
                var status = string.Equals(u, from, StringComparison.OrdinalIgnoreCase) ? statusRead : statusNew;
                uofw.TopicUsers.Insert(new TopicUser { TopicMessage = newMessage, User = u, TopicStatus = status });
            }

            if (attachments != null)
            {
                foreach (var a in attachments)
                {
                    uofw.TopicAttachments.Insert(new TopicAttachment { FileName = a.FileName, OidDocument = a.DocumentOid, TopicMessage = newMessage });
                }
            }
        }

        /// <summary>
        /// Create TopicInfo
        /// </summary>
        /// <param name="user">TopicUser</param>
        /// <param name="uofw">FlowTasksUnitOfWork</param>
        /// <param name="num">Number of records to return</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="status"></param>
        /// <param name="withReplies"></param>
        /// <returns>TopicInfo</returns>
        private TopicInfo CreateTopicInfo(TopicUser user, FlowTasksUnitOfWork uofw, int num, DateTime start, DateTime end, string status, bool withReplies)
        {
            var replies = new List<TopicMessageInfo>();
            var statusNewStr = TopicStatusType.New.ToString();
            var statusReadStr = TopicStatusType.Read.ToString();
            var statusRead = uofw.TopicStatuses.First(s => s.Status == statusReadStr);
            var globalTopicStatus = CreateStatusType(user);

            IEnumerable<TopicUser> userTopics;
            if (withReplies)
            {
                userTopics =
                    uofw.TopicUsers.Find(
                        tu =>
                            !tu.TopicMessage.IsTopic && tu.TopicMessage.TopicId == user.TopicMessage.TopicId &&
                            tu.User == user.User
                            && (string.IsNullOrEmpty(status) || tu.TopicStatus.Status == status),
                        tu => tu.TopicMessage, tu => tu.TopicMessage.Topic, tu => tu.TopicStatus).ToList();
            }
            else
            {
                // If we don't need the replyes, just load the new topics because we want to change the status
                userTopics =
                    uofw.TopicUsers.Find(
                        tu =>
                            !tu.TopicMessage.IsTopic && tu.TopicMessage.TopicId == user.TopicMessage.TopicId &&
                            tu.User == user.User && tu.TopicStatus.Status.Equals(statusNewStr),
                        tu => tu.TopicMessage, tu => tu.TopicMessage.Topic, tu => tu.TopicStatus).ToList();                
            }

            var users = new Dictionary<string, string>();

            foreach (var r in userTopics)
            {
                if (withReplies)
                {
                    replies.Add(CreateMessageInfo(r, uofw, users));
                }

                if (r.TopicStatus.Status == statusNewStr)
                {
                    globalTopicStatus = TopicStatusType.New;
                    r.TopicStatus = statusRead;
                }

            }

            var ti = new TopicInfo { TopicId = user.TopicMessage.TopicId, Title = user.TopicMessage.Topic.Title, Status = globalTopicStatus, Message = CreateMessageInfo(user, uofw, users), Replies = replies.ToArray() };

            return ti;
        }

        /// <summary>
        /// Create MessageInfo
        /// </summary>
        /// <param name="user">TopicUser</param>
        /// <param name="uofw">FlowTasksUnitOfWork</param>
        /// <param name="users"></param>
        /// <returns>TopicMessageInfo</returns>
        private TopicMessageInfo CreateMessageInfo(TopicUser user, FlowTasksUnitOfWork uofw, Dictionary<string, string> users)
        {
            var documents = CreateDocuments(user.TopicMessage, uofw);

            if (!users.ContainsKey(user.TopicMessage.From))
            {
                GetUserResponse resp;
                if (_usersService == null)
                {
                    using (var usersOperations = new FlowUsersService())
                    {
                        resp = usersOperations.GetUser(new GetUserRequest {User = user.TopicMessage.From});
                    }
                }
                else
                    resp = _usersService.GetUser(new GetUserRequest {User = user.TopicMessage.From});

                users.Add(user.TopicMessage.From, resp.User.PhotoPath);
            }

            return new TopicMessageInfo
            {
                From = user.TopicMessage.From,
                ImageUrl = users[user.TopicMessage.From],
                To = user.TopicMessage.To,
                Message = user.TopicMessage.Message,
                When = user.TopicMessage.Topic.LastChanged,
                Status = CreateStatusType(user),
                Attachments = documents
            };
        }

        /// <summary>
        /// Create Documents
        /// </summary>
        /// <param name="m">TopicMessage</param>
        /// <param name="uofw">FlowTasksUnitOfWork</param>
        /// <returns>List of TopicAttachmentInfo</returns>
        private IEnumerable<TopicAttachmentInfo> CreateDocuments(TopicMessage m, FlowTasksUnitOfWork uofw)
        {
            var documents = new List<TopicAttachmentInfo>();
            foreach (var d in uofw.TopicAttachments.Find(a => a.TopicMessageId == m.TopicMessageId))
            {
                documents.Add(new TopicAttachmentInfo { FileName = d.FileName, DocumentOid = d.OidDocument });
            }
            return documents;
        }

        /// <summary>
        /// Create Status Type
        /// </summary>
        /// <param name="user">TopicUser</param>
        /// <returns>TopicStatusType</returns>
        private TopicStatusType CreateStatusType(TopicUser user)
        {
            return (TopicStatusType)Enum.Parse(typeof(TopicStatusType), user.TopicStatus.Status);
        }

        /// <summary>
        /// Get Broadcast Users
        /// </summary>
        /// <param name="from">From</param>
        /// <returns>users</returns>
        private string GetBroadcastUsers(string from)
        {
            GetUserResponse resp;
            if (_usersService == null)
            {
                using (var usersOperations = new FlowUsersService())
                {
                    resp = usersOperations.GetUser(new GetUserRequest { User = from });
                }
            }
            else
                resp = _usersService.GetUser(new GetUserRequest { User = from });

            if (resp.User.FollowerCount == 0)
            {
                return string.Empty;
            }

            return resp.User.Follower
                .Select(f => f.UserName)
                .Aggregate((current, next) => current + ";" + next);
        }

        /// <summary>
        /// Topics For User
        /// </summary>
        /// <param name="uofw">Uofw</param>
        /// <param name="user">User</param>
        /// <param name="topicId">TopicId</param>
        /// <param name="title">Title</param>
        /// <param name="status"></param>
        /// <param name="withReplies"></param>
        /// <returns></returns>
        private IQueryable<TopicUser> TopicsForUser(IFlowTasksUnitOfWork uofw, string user, int topicId, string title, string status, bool withReplies)
        {
            var q = from tu in uofw.TopicUsers.AsQueryable()
                    .Include("TopicStatus")
                    .Include("TopicMessage")
                    .Include("TopicMessage.Topic")
                    where (withReplies || tu.TopicMessage.IsTopic) && tu.User.Equals(user, StringComparison.OrdinalIgnoreCase) &&
                          (topicId == 0 || tu.TopicMessage.Topic.TopicId == topicId) &&
                          (string.IsNullOrEmpty(title) || tu.TopicMessage.Topic.Title.Contains(title)) &&
                          (string.IsNullOrEmpty(status) || tu.TopicStatus.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                    select tu;

            return q;
        }

        /// <summary>
        /// Convert user search pattern to a full test search. "or" and "and" are translated to 
        /// OR, AND full text keywords.
        /// </summary>
        /// <param name="pattern">Pattern</param>
        /// <returns>Full text search string</returns>
        private static string ConvertPatternFullTextSearch(string pattern)
        {
            var splitOr = pattern.Split(new[] { "or" }, StringSplitOptions.RemoveEmptyEntries);
            var fullTextPattern = new StringBuilder();

            var count = 0;
            foreach (var or in splitOr)
            {
                var splitAnd = or.Split(new[] { "and" }, StringSplitOptions.RemoveEmptyEntries).Select(s => string.Format("\"{0}\"", s.Trim()));
                fullTextPattern.Append(string.Join(" AND ", splitAnd));
                count += 1;
                if (count < splitOr.Length)
                {
                    fullTextPattern.Append(" OR ");
                }
            }

            return fullTextPattern.ToString();
        }

    }
}
