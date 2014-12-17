using System.Reflection;
using System.Web.Http;
using Flow.Users.Contract;
using log4net;

namespace Flow.Tasks.Web.Controllers.Api
{
    public class LoggingController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        

        /// <summary>
        /// Get User
        /// </summary>
        /// <remarks>
        /// http://localhost/Flow.tasks.web/api/logging
        /// </remarks>
        [HttpPost]
        public void Logging([FromBody]MessageFormat message)
        {
            if (message == null) return;

            Log.Debug(message.Message);
        }

        public class MessageFormat
        {
            public string Message { get; set; }
        }

    }
}