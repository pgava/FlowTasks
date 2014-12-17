using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

namespace Flow.Users.Contract.Message
{
    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Gender { get; set; }

        [DataMember]
        public DateTime? Birthday { get; set; }

        [DataMember]
        public string BirthdayStr { get; set; }

        [DataMember]
        public string PhotoPath { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Position { get; set; }

        [DataMember]
        public string Department { get; set; }

        [DataMember]
        public IEnumerable<UserInfo> Following { get; set; }

        [DataMember]
        public IEnumerable<UserInfo> Follower { get; set; }

        [DataMember]
        public int TasksToDoCount { get; set; }

        [DataMember]
        public int TasksCompletedCount { get; set; }

        [DataMember]
        public int FollowingCount { get; set; }

        [DataMember]
        public int FollowerCount { get; set; }

        [DataMember]
        public IEnumerable<UserPages> UserPages { get; set; }

    }

    [DataContract]
    public class UserPages
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public int Order { get; set; }

    }
}
