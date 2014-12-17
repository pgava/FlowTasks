using MvcContrib.PortableAreas;
using System.Web.Mvc;
using Holiday.Views.HolidayUserView.Messages;

namespace Holiday.Views.HolidayUserView
{
    public class HolidayUserViewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering HolidayUserView Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "HolidayUserView",
                "HolidayUserView/{controller}/{action}",
                new { controller = "HolidayUserView", action = "index" });
        }

        public override string AreaName
        {
            get { return "HolidayUserView"; }
        }
    }
}
