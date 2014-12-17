using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class WorkflowCode
    {
        public int WorkflowCodeId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Code { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
