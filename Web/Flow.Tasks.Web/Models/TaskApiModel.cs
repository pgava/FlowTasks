namespace Flow.Tasks.Web.Models
{
    public class TaskApiModel
    {
        public string Url{ get; set; }

        public View.Models.TaskModel Task { get; set; }
    }
}