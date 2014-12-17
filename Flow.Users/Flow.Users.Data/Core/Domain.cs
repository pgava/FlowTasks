using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Users.Data.Core
{
    public class Domain
    {
        public int DomainId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(20)]
        public string Fax { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }

        public int? AddressDetailsId { get; set; }
        public AddressDetails AddressDetails { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}
