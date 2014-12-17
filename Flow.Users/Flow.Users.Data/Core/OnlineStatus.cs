using System.ComponentModel.DataAnnotations;
namespace Flow.Users.Data.Core
{
    public class OnlineStatus
    {
        public int OnlineStatusId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Code { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
