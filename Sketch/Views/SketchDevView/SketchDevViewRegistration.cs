using MvcContrib.PortableAreas;
using System.Web.Mvc;
using Sketch.Views.SketchDevView.Messages;

namespace Sketch.Views.SketchDevView
{
    public class SketchDevViewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering SketchDevView Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "SketchDevView",
                "SketchDevView/{controller}/{action}",
                new { controller = "SketchDevView", action = "index" });
        }

        public override string AreaName
        {
            get { return "SketchDevView"; }
        }
    }
}
