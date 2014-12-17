namespace Flow.Users.Data.Core
{
    public class UserFollowing
    {
        public int UserFollowingId { get; set; }

        public int FollowerUserId { get; set; }
        public User FollowerUser { get; set; }

        public int FollowingUserId { get; set; }
        public User FollowingUser { get; set; }

    }
}
