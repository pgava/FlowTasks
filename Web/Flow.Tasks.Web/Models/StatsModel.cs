using System.Collections.Generic;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Web.Models
{
    public class StatsModel
    {
        public int TasksCompletedCount { get; set; }
     
        public int TasksToDoCount { get; set; }

        public IEnumerable<TasksOn> TasksCompleted { get; set; }

        public IEnumerable<TasksOn> TasksToDo { get; set; }
    }
}