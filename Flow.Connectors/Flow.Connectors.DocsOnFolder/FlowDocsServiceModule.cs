using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Flow.Docs.Contract;
using Flow.Docs.Proxy;
using Flow.Tasks.Contract;
using Flow.Tasks.Proxy;
using Flow.Docs.Contract.Interface;
using Flow.Docs.Process;

namespace Flow.Connectors.DocsOnFolder
{
    public class FlowDocsServiceModule : NinjectModule 
    {
        public override void Load()
        {
            //Bind<IFlowTasksService>().To<FlowTasksService>()
            //    .WithConstructorArgument("endpointConfigurationName", "FlowTasksService_Endpoint");

            Bind<IFlowDocsDocument>().To<FlowDocsDocument>();
        } 
    }
}
