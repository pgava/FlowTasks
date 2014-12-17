using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class Property
    {
        public int PropertyId { get; set; }
        [MaxLength(200)]
        [Required]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Value { get; set; }
        [MaxLength(20)]
        public string Type { get; set; }
    }
}
