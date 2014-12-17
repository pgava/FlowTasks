using System;
using Flow.Tasks.Contract.Message;
using System.Collections.Generic;

namespace Flow.Tasks.Contract.Interface
{
    /// <summary>
    /// Topic Interface
    /// </summary>
    public interface ITopic
    {
        /// <summary>
        /// Get Topics For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="topicId"></param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="withReplies"></param>
        /// <returns>List of TopicInfo</returns>
        IEnumerable<TopicInfo> GetTopicsForUser(string user, int topicId, DateTime start, DateTime end, string title, string status, int pageIndex, int pageSize, bool withReplies);

        /// <summary>
        /// Get Replies For User
        /// </summary>
        /// <param name="topicId">TopicId</param>
        /// <param name="user">User</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="showType"></param>
        /// <returns>List of TopicMessageInfo</returns>
        IEnumerable<TopicMessageInfo> GetRepliesForUser(int topicId, string user, DateTime start, DateTime end, RepliesShowType showType, out bool hasOldReplies);

        /// <summary>
        /// Get Topic Count
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Number of topic unread</returns>
        int GetTopicCount(string user);

        /// <summary>
        /// Search For Topics
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="pattern">Pattern</param>
        /// <returns>List of topics that match pattern</returns>
        IEnumerable<SearchInfo> SearchForTopics(string user, string pattern);

        /// <summary>
        /// Create Topic
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="attachments">Attachments</param>
        int CreateTopic(string title, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments);

        /// <summary>
        /// Create Reply
        /// </summary>
        /// <param name="topicId">TopicId</param>
        /// <param name="message">Message</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="attachments">Attachments</param>
        void CreateReply(int topicId, string message, string from, string to, IEnumerable<TopicAttachmentInfo> attachments); 
    }
}
