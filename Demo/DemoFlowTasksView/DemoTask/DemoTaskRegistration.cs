using MvcContrib.PortableAreas;
using System.Web.Mvc;
using DemoFlowTasksView.DemoTask.Messages;

namespace DemoFlowTasksView.DemoTask
{
    public class DemoTaskRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering Demo Task Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "DemoTask",
                "DemoTask/{controller}/{action}",
                new { controller = "DemoTask", action = "index" });
        }

        public override string AreaName
        {
            get { return "DemoTask"; }
        }
    }
}
