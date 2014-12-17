using MvcContrib.PortableAreas;
using Flow.Tasks.View.ApproveTask.Messages;
using System.Web.Mvc;

namespace Flow.Tasks.View.ApproveTask
{
    public class ApproveTaskRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering Approve Task Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "ApproveTask",
                "ApproveTask/{controller}/{action}",
                new { controller = "ApproveTask", action = "index" });

            //RegisterTheViewsInTheEmbeddedViewEngine(GetType());
        }

        public override string AreaName
        {
            get { return "ApproveTask"; }
        }
    }
}
