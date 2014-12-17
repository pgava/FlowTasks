using MvcContrib.PortableAreas;
using System.Web.Mvc;
using HrView.MgrView.Messages;

namespace HrView.MgrView
{
    public class MgrViewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering MgrView Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "MgrView",
                "MgrView/{controller}/{action}",
                new { controller = "MgrView", action = "index" });
        }

        public override string AreaName
        {
            get { return "MgrView"; }
        }
    }
}
