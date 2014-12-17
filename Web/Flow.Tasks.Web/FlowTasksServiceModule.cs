using Ninject.Modules;
using Flow.Tasks.Contract;
using Flow.Tasks.Proxy;
using Flow.Docs.Contract;
using Flow.Docs.Proxy;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Process;
using Flow.Users.Contract;
using Flow.Users.Proxy;
using System.Web;

namespace Flow.Tasks.Web
{
    public class FlowTasksServiceModule : NinjectModule 
    { 
        public override void Load() 
        {
            Bind<IFlowTasksService>().To<FlowTasksService>()
                .InScope(ctx => HttpContext.Current)
                .WithConstructorArgument("endpointConfigurationName", "FlowTasksService_Endpoint");

            Bind<IFlowUsersService>().To<FlowUsersService>()
                .InScope(ctx => HttpContext.Current)
                .WithConstructorArgument("endpointConfigurationName", "FlowUsersService_Endpoint");

            Bind<IFlowDocsDocument>().To<FlowDocsDocument>()
                .InScope(ctx => HttpContext.Current);

        } 
    }
}