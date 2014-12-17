using Ninject.Modules;
using Flow.Tasks.Contract;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Data.DAL;
using Flow.Tasks.Proxy;

namespace Flow.Tasks.Service
{
    public class FlowTasksServiceModule : NinjectModule 
    { 
        public override void Load() 
        {
            //======================================================================
            // NInject -> Passing arguments to ctor
            //Bind<IFlowUsersService>().To<FlowUsersService>()
            //      .InScope(ctx => System.ServiceModel.Web.WebOperationContext.Current)
            //      .WithConstructorArgument("endpointConfigurationName", "FlowUsersService_Endpoint");
            //======================================================================
                
            Bind<ITask>().To<Task>();
            Bind<ITracer>().To<Tracer>();
            Bind<IWorkflow>().To<Workflow>();
            Bind<ITopic>().To<Topic>();
            Bind<IHoliday>().To<Holiday>();
            Bind<IFlowTasksProxyManager>().To<FlowTasksProxyManager>();
        } 
    }
}