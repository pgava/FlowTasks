using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Flow.Users.Data.Core
{
    public class DomainUser
    {
        public int DomainUserId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int DomainId { get; set; }
        public Domain Domain { get; set; }
    }
}
