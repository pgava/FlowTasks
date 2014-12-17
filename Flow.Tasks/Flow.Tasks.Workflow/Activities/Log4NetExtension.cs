using System;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Flow.Library;

namespace Flow.Tasks.Workflow.Activities
{
    public class Log4NetExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get
            {
                return typeof(Log4NetExtensionBehavior);
            }
        }
        protected override object CreateBehavior()
        {
            return new Log4NetExtensionBehavior();
        }
    }

    public class Log4NetExtensionBehavior : IServiceBehavior
    {
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var host = (WorkflowServiceHost)serviceHostBase;
            host.WorkflowExtensions.Add(() => new Log4NetStartup());

        }

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }

        #endregion
    }

    public class Log4NetStartup
    {
        public Log4NetStartup()
        {
            LoggerUtil.ConfigureLogging();
        }
    }
}
