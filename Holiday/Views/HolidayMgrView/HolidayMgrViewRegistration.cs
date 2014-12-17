using MvcContrib.PortableAreas;
using System.Web.Mvc;
using Holiday.Views.HolidayMgrView.Messages;

namespace Holiday.Views.HolidayMgrView
{
    public class HolidayMgrViewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering HolidayMgrView Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "HolidayMgrView",
                "HolidayMgrView/{controller}/{action}",
                new { controller = "HolidayMgrView", action = "index" });
        }

        public override string AreaName
        {
            get { return "HolidayMgrView"; }
        }
    }
}
