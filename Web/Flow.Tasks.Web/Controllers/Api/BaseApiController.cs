using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;
using System.Web.Http;
using Flow.Tasks.Web.Models;
using Flow.Users.Contract;

namespace Flow.Tasks.Web.Controllers.Api
{
    [Authorize]
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// FlowTasksService
        /// </summary>
        protected IFlowTasksService TasksService;

        /// <summary>
        /// FlowTasksService
        /// </summary>
        protected IFlowUsersService UsersService;

        /// <summary>
        /// FlowDocsDocument
        /// </summary>
        protected IFlowDocsDocument DocsDocument;

        /// <summary>
        /// ApiModelFactory
        /// </summary>
        private ApiModelFactory _apiModelFactory;

        /// <summary>
        /// ApiModelFactory
        /// </summary>
        protected ApiModelFactory TheModelFactory
        {
            get { return _apiModelFactory ?? (_apiModelFactory = new ApiModelFactory(Request)); }
        }

        public BaseApiController(IFlowTasksService tasksService)
        {
            TasksService = tasksService;
        }

        public BaseApiController(IFlowUsersService usersService)
        {
            UsersService = usersService;
        }

        public BaseApiController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument)
        {
            TasksService = tasksService;
            DocsDocument = docsDocument;
        }
        
        public BaseApiController(IFlowUsersService usersService, IFlowDocsDocument docsDocument)
        {
            UsersService = usersService;
            DocsDocument = docsDocument;
        }

        public BaseApiController(IFlowTasksService tasksService, IFlowDocsDocument docsDocument, IFlowUsersService usersService)
        {
            TasksService = tasksService;
            DocsDocument = docsDocument;
            UsersService = usersService;
        }

    }
}