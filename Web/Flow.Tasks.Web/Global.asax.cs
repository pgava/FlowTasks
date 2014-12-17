using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Flow.Library;
using MvcContrib.PortableAreas;
using System.Web.Security;
using Flow.Library.Security;
using System.Web.Http;
using Flow.Tasks.Web.App_Start;
//using System.Globalization;

namespace Flow.Tasks.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {        

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LoggerUtil.ConfigureLogging();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[
                     FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                //Extract the forms authentication cookie 
                FormsAuthenticationTicket authTicket =
                       FormsAuthentication.Decrypt(authCookie.Value);
                
                FlowTasksIdentity id = new FlowTasksIdentity(authTicket.Name, authTicket.UserData);
                FlowTasksPrincipal user = new FlowTasksPrincipal(id);
                Context.User = user;

                /*
                 * When you are going to use the information later, you can access your custom principal as follows. 
                     (CustomPrincipal)this.User 
                   or  
                     (CustomPrincipal)this.Context.User 
                */
            }
        }

        //protected void Application_BeginRequest(Object sender, EventArgs e)
        //{
        //    var culture = new CultureInfo("en-AU", false);
        //    culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        //    culture.DateTimeFormat.DateSeparator = "/";
        //    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        //}
    }
}