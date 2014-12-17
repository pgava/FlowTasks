using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Users.Data.Core
{
    public class User
    {
        public int UserId { get; set; }

        [MaxLength(16)]
        [Required]
        public string Name { get; set; }        
        [MaxLength(256)]
        [Required]
        public string Email { get; set; }
        [MaxLength(100)]
        [Required]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(5)]
        public string Title { get; set; }
        [MaxLength(5)]
        public string Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public byte[] Photo { get; set; }
        [MaxLength(256)]
        public string PhotoPath { get; set; }
        [MaxLength(600)]
        public string Note { get; set; }
        [MaxLength(20)]
        public string HomePhone { get; set; }
        [MaxLength(20)]
        public string MobilePhone { get; set; }
        [MaxLength(20)]
        public string WorkPhone { get; set; }
        [MaxLength(50)]
        public string Position { get; set; }
        [MaxLength(50)]
        public string Department{ get; set; }
        public DateTime? HireDate{ get; set; }

        public int? OnlineStatusId { get; set; }
        public OnlineStatus OnlineStatus { get; set; }

        public int? AddressDetailsId { get; set; }
        public AddressDetails AddressDetails { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}
