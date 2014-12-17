using System.Web.Http;
using Flow.Library;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Web.Models;

namespace Flow.Tasks.Web.Controllers.Api
{

    public class UserStatsController : BaseApiController
    {
        public UserStatsController(IFlowTasksService tasksService)
            : base(tasksService)
        {
        }

        /// <summary>
        /// Get Stats
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/users/stats/cgrant
        /// </remarks>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public StatsModel GetStats(string name)
        {
            var stats = TasksService.GetStatsForUser(new GetStatsForUserRequest {User = name});

            return new StatsModel {TasksCompletedCount = stats.TaskCompleted, TasksToDoCount = stats.TaskToDo, TasksCompleted = stats.TasksCompleted, TasksToDo = stats.TasksToDo};
        }

    }
}