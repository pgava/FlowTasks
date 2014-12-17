namespace Flow.Users.Data.Core
{
    public class RoleUser
    {
        public int RoleUserId { get; set; }

        public bool IsPrimary { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
