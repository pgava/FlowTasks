using Flow.Tasks.Contract.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flow.Tasks.Web.Models
{
    /// <summary>
    /// Topics Model
    /// </summary>
    public class TopicsModel
    {
        /// <summary>
        /// List of topics
        /// </summary>
        public IEnumerable<TopicModelSubject> Subjects { get; set; }

        public TopicsModel() { }

        public TopicsModel(IEnumerable<TopicInfo> topics)
        {
            var subjetcs = new List<TopicModelSubject>();
            if (topics != null)
            {
                foreach (var t in topics)
                {
                    subjetcs.Add(new TopicModelSubject(t));
                }
            }
            Subjects = subjetcs;
        }
    }

    /// <summary>
    /// Topic Model Subject
    /// </summary>
    public class TopicModelSubject
    {
        /// <summary>
        /// Status: Read/New
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Topic Id
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public TopicModelItem Message { get; set; }

        /// <summary>
        /// Replies
        /// </summary>
        public IEnumerable<TopicModelItem> Replies { get; set; }

        public TopicModelSubject() { }
        public TopicModelSubject(TopicInfo topic)
        {
            Status = topic.Status.ToString();
            Title = topic.Title;
            TopicId = topic.TopicId;
            Message = new TopicModelItem(topic.Message);

            var replies = new List<TopicModelItem>();
            foreach (var r in topic.Replies)
            {
                replies.Add(new TopicModelItem(r));
            }
            Replies = replies;
        }

    }

    /// <summary>
    /// Topics Model
    /// </summary>
    public class TopicRepliesModel
    {
        /// <summary>
        /// List of replies
        /// </summary>
        public IEnumerable<TopicModelItem> Replies { get; set; }

        /// <summary>
        /// TopicId
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Has Old Replies
        /// </summary>
        public bool HasOldReplies { get; set; }

        public TopicRepliesModel() { }

        public TopicRepliesModel(int topicId, IEnumerable<TopicMessageInfo> replies, bool hasOldReplies)
        {
            TopicId = topicId;
            
            HasOldReplies = hasOldReplies;

            var items = new List<TopicModelItem>();
            if (replies != null)
            {
                foreach (var r in replies)
                {
                    items.Add(new TopicModelItem(r));
                }
            }
            Replies = items;
        }
    }


    /// <summary>
    /// Topic Model Item
    /// </summary>
    public class TopicModelItem
    {
        /// <summary>
        /// Status Read/New
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Image Url
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// When Time
        /// </summary>
        public string WhenTime { get; set; }

        /// <summary>
        /// When Time
        /// </summary>
        public string WhenDay { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Attachments
        /// </summary>
        public IEnumerable<AttachmentModel> Attachments { get; set; }

        public TopicModelItem() { }

        public TopicModelItem(TopicMessageInfo message)
        {
            Status = message.Status.ToString();
            ImageUrl = message.ImageUrl;
            From = message.From;
            To = message.To;
            WhenTime = message.When.ToString("T");
            WhenDay = message.When.ToString("D");
            Message = message.Message;
            Attachments = message.Attachments.Select(d => new AttachmentModel { FileName = d.FileName, Oid = d.DocumentOid.ToString() }).ToList();
        }
    }

    /// <summary>
    /// Attachment Model
    /// </summary>
    public class AttachmentModel
    {
        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Oid
        /// </summary>
        public string Oid { get; set; }
    }

}