using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using Flow.Tasks.View;
using Flow.Tasks.Web.Models;
using Flow.Users.Contract;
using System.Web.Mvc;

namespace Flow.Tasks.Web.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    [HandleError]
    public class HomeController : BaseController
    {
        public HomeController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        /// <summary>
        /// Index Action
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Home";

            return View(new HomeModel
            {
                TaskCount = 0
            });
        }

        
    }
}
