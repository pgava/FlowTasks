using System.ComponentModel.DataAnnotations;
namespace Flow.Users.Data.Core
{
    public class AddressDetails
    {
        public int AddressDetailsId { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(50)]
        public string Region { get; set; }
        [MaxLength(20)]
        public string PostalCode { get; set; }
        [MaxLength(50)]
        public string Country { get; set; }
    }
}
