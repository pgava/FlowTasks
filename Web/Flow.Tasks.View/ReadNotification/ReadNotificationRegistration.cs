using MvcContrib.PortableAreas;
using Flow.Tasks.View.ReadNotification.Messages;
using System.Web.Mvc;

namespace Flow.Tasks.View.ReadNotification
{
    public class ReadNotificationRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering Read Notification Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "ReadNotification",
                "ReadNotification/{controller}/{action}",
                new { controller = "ReadNotification", action = "index" });

            //RegisterTheViewsInTheEmbeddedViewEngine(GetType());
        }

        public override string AreaName
        {
            get { return "ReadNotification"; }
        }
    }
}
