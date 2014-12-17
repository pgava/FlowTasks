using Ninject.Modules;
using Flow.Users.Contract.Interface;
using Flow.Users.Data.DAL;

namespace Flow.Users.Service
{
    public class FlowUsersOperationsModule : NinjectModule 
    { 
        public override void Load() 
        {
            Bind<IFlowUsersOperations>().To<FlowUsersOperations>();
        } 
    }
}