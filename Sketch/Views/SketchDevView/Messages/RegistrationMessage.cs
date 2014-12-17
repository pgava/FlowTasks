using MvcContrib.PortableAreas;

namespace Sketch.Views.SketchDevView.Messages
{
    public class RegistrationMessage : IEventMessage
    {
        public RegistrationMessage(string message)
        {
            _message = message;
        }

        private readonly string _message;

        public override string ToString()
        {
            return _message;
        }
    }
}
