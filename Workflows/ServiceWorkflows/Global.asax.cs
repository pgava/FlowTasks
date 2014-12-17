using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ServiceWorkflows
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start(object sender, EventArgs e)
        {
            Log.Debug("Application_Start -> Start");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Log.Debug("Session_Start -> Start");

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Log.Debug("Application_BeginRequest -> Start");

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            Log.Debug("Application_AuthenticateRequest -> Start");

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Log.Debug("Application_Error -> Start");

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Log.Debug("Session_End -> Start");

        }

        protected void Application_End(object sender, EventArgs e)
        {
            Log.Debug("Application_End -> Start");

        }
    }
}