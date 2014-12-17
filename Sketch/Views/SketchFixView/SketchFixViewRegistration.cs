using MvcContrib.PortableAreas;
using System.Web.Mvc;
using Sketch.Views.SketchFixView.Messages;

namespace Sketch.Views.SketchFixView
{
    public class SketchFixViewRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            bus.Send(new RegistrationMessage("Registering SketchDevView Portable Area"));

            base.RegisterArea(context, bus);

            context.MapRoute(
                "SketchFixView",
                "SketchFixView/{controller}/{action}",
                new { controller = "SketchFixView", action = "index" });
        }

        public override string AreaName
        {
            get { return "SketchFixView"; }
        }
    }
}
