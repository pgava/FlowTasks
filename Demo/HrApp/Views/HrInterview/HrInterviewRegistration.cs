using MvcContrib.PortableAreas;
using System.Web.Mvc;
using HrView.HrInterview.Messages;

namespace HrView.HrInterview
{
    public class HrInterviewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering HrInterview Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "HrInterview",
                "HrInterview/{controller}/{action}",
                new { controller = "HrInterview", action = "index" });
        }

        public override string AreaName
        {
            get { return "HrInterview"; }
        }
    }
}
