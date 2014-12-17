using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Users.Data.Core
{
    public class Role
    {
        public int RoleId { get; set; }
        [MaxLength(20)]
        [Required]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }        
    }
}
