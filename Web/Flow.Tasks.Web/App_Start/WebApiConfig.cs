using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Flow.Tasks.Web.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "WorkflowOperations",
                routeTemplate: "api/workflows/{op}/{woid}",
                defaults: new { controller = "workflowoperations", woid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DashboardDetails",
                routeTemplate: "api/dashboarddetails/{woid}",
                defaults: new { controller = "dashboarddetails" }
            );

            config.Routes.MapHttpRoute(
                name: "DashboardWorkflows",
                routeTemplate: "api/dashboardworkflows/{woid}",
                defaults: new { controller = "dashboardworkflows", woid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Comments",
                routeTemplate: "api/comments/{woid}",
                defaults: new { controller = "comments" }
            );

            config.Routes.MapHttpRoute(
                name: "TaskOperation",
                routeTemplate: "api/tasks/{toid}/{op}",
                defaults: new { controller = "taskoperations" }
            );

            config.Routes.MapHttpRoute(
                name: "Task",
                routeTemplate: "api/tasks/{toid}",
                defaults: new { controller = "tasks", toid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "UserStats",
                routeTemplate: "api/users/stats/{name}",
                defaults: new { controller = "userstats" }
            );

            config.Routes.MapHttpRoute(
                name: "Holiday",
                routeTemplate: "api/users/holiday/{name}",
                defaults: new { controller = "holiday" }
            );

            config.Routes.MapHttpRoute(
                name: "Avatar",
                routeTemplate: "api/users/avatar/{name}",
                defaults: new { controller = "avatar" }
            );

            config.Routes.MapHttpRoute(
                name: "UserFollowing",
                routeTemplate: "api/users/follows/{name}/{following}",
                defaults: new { controller = "userfollowing" }
            );

            config.Routes.MapHttpRoute(
                name: "Password",
                routeTemplate: "api/users/password/{name}/{oldp}/{newp}",
                defaults: new { controller = "password" }
            );

            config.Routes.MapHttpRoute(
                name: "User",
                routeTemplate: "api/users/{name}",
                defaults: new { controller = "users", name = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Reply",
                routeTemplate: "api/replies/{tid}",
                defaults: new { controller = "replies"}
            );

            config.Routes.MapHttpRoute(
                name: "Topic",
                routeTemplate: "api/topics/{id}",
                defaults: new { controller = "topics", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Doc",
                routeTemplate: "api/docs/{oid}",
                defaults: new { controller = "docs"}
            );

            config.Routes.MapHttpRoute(
                name: "Reports",
                routeTemplate: "api/reports/{action}",
                defaults: new { controller = "reports" }
            );

            config.Routes.MapHttpRoute(
                name: "Sketch",
                routeTemplate: "api/sketch/{fname}",
                defaults: new { controller = "sketch" }
            );

            config.Routes.MapHttpRoute(
                name: "SketchAction",
                routeTemplate: "api/sketch/op/{action}",
                defaults: new { controller = "sketch" }
            );

            config.Routes.MapHttpRoute(
                name: "Logging",
                routeTemplate: "api/logging",
                defaults: new { controller = "logging" }
            );

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
