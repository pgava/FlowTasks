using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class Holiday
    {
        public int HolidayId { get; set; }

        [MaxLength(16)]
        public string User { get; set; }

        [Required]
        public int Year { get; set; }

        [MaxLength(1)]
        [Required]
        public string Status { get; set; }

        public int HolidayTypeId { get; set; }
        public HolidayType HolidayType { get; set; }

        [MaxLength(200)]
        [Required]
        public string Dates { get; set; }
    }
}
