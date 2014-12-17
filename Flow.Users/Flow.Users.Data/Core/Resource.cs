using System.ComponentModel.DataAnnotations;

namespace Flow.Users.Data.Core
{
    public class Resource
    {
        public int ResourceId { get; set; }

        [MaxLength(20)]
        public string Type { get; set; }
        [MaxLength(20)]
        public string Display { get; set; }
        [MaxLength(100)]
        public string Value { get; set; }
        public int Order { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }
}
