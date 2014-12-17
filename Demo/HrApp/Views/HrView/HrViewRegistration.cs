using MvcContrib.PortableAreas;
using System.Web.Mvc;
using HrView.HrView.Messages;

namespace HrView.HrView
{
    public class HrViewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering HrView Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "HrView",
                "HrView/{controller}/{action}",
                new { controller = "HrView", action = "index" });
        }

        public override string AreaName
        {
            get { return "HrView"; }
        }
    }
}
