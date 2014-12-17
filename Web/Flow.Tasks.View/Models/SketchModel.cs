using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.View.Models
{
    /// <summary>
    /// TaskModel
    /// </summary>
    public class SketchModel 
    {
        [Display(Name = "Workflow")]
        public string Workflow { get; set; }

        public TaskModel RedirectTask { get; set; }
    }


}