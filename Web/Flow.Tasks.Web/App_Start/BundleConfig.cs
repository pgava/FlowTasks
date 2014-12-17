using System.Web;
using System.Web.Optimization;

namespace Flow.Tasks.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            // Adding specific bundle for application

            bundles.Add(new StyleBundle("~/Content/themes/smart/css/css").Include(
                "~/Content/themes/smart/css/bootstrap.css",
                "~/Content/themes/smart/css/smartadmin-production.min.css",
                "~/Content/themes/smart/css/smartadmin-skins.min.css",
                "~/Content/themes/smart/css/custom.css",
                "~/Content/themes/smart/css/demo.css",
                "~/Content/themes/smart/js/datepicker/css/pepper-ginder-custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/smart").Include(
                "~/Content/themes/smart/js/bootstrap/bootstrap.js",
                "~/Content/themes/smart/js/notification/SmartNotification.js",
                "~/Content/themes/smart/js/plugin/sparkline/jquery.sparkline.js",
                "~/Content/themes/smart/js/plugin/msie-fix/jquery.mb.browser.js",
                "~/Content/themes/smart/js/app.js",
                "~/Content/Themes/smart/js/file-upload/9.5.7/jquery.fileupload.js",
                "~/Content/Themes/smart/js/datepicker/jquery-ui.multidatespicker.js",
                "~/Content/Themes/smart/js/datetime/moment.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/flowTasksApp/js/angular.min.js",
                "~/Scripts/flowTasksApp/js/angular-ui-router.min.js",
                "~/Scripts/flowTasksApp/js/angular-resource.min.js",
                "~/Scripts/flowTasksApp/js/angular-sanitize.min.js",
                "~/Scripts/flowTasksApp/js/ui-utils.min.js",
                "~/Scripts/flowTasksApp/js/ng-infinite-scroll.min.js",
                "~/Scripts/flowTasksApp/js/ng-table.js",
                "~/Scripts/flowTasksApp/js/angular-webstorage.js",
                "~/Scripts/flowTasksApp/js/ng-google-chart.js",
                "~/Scripts/flowTasksApp/js/angular-touch.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app-config").Include(
                "~/Scripts/global.js",
                "~/Scripts/flowTasksApp/flowTasksApp.js",
                "~/Scripts/flowTasksApp/router.js"));

            bundles.Add(new ScriptBundle("~/bundles/app-services").Include(
                // Added By New Ocean
                // Factories
                "~/Scripts/flowTasksApp/factories/AlertFactory.js",
                "~/Scripts/flowTasksApp/factories/ApplicationFactory.js",
                "~/Scripts/flowTasksApp/factories/TaskFactory.js",
                "~/Scripts/flowTasksApp/factories/TopicFactory.js",
                "~/Scripts/flowTasksApp/factories/HolidayFactory.js",
                "~/Scripts/flowTasksApp/factories/SketchFactory.js",
                // Services
                "~/Scripts/flowTasksApp/services/WorkContextService.js",
                "~/Scripts/flowTasksApp/services/LoggingService.js",
                "~/Scripts/flowTasksApp/services/TaskService.js",
                "~/Scripts/flowTasksApp/services/AuthenticationService.js",
                "~/Scripts/flowTasksApp/services/UserService.js",
                "~/Scripts/flowTasksApp/services/TopicService.js",
                "~/Scripts/flowTasksApp/services/HolidayService.js",
                "~/Scripts/flowTasksApp/services/DashboardService.js",
                "~/Scripts/flowTasksApp/services/ReportService.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/app-directives").Include(
                "~/Scripts/flowTasksApp/directives/MainDirective.js",
                "~/Scripts/flowTasksApp/directives/TopicDirective.js",
                "~/Scripts/flowTasksApp/directives/UserDirective.js",
                "~/Scripts/flowTasksApp/directives/TaskDirective.js",
                // Added By New Ocean
                "~/Scripts/flowTasksApp/directives/HolidayDirective.js",
                "~/Scripts/flowTasksApp/directives/ReportDirective.js",
                "~/Scripts/flowTasksApp/directives/SketchDirective.js",
                "~/Scripts/flowTasksApp/directives/DashboardDirective.js"));

            bundles.Add(new ScriptBundle("~/bundles/app-controllers").Include(
                "~/Scripts/flowTasksApp/controllers/AppController.js",
                "~/Scripts/flowTasksApp/controllers/HomeController.js",
                "~/Scripts/flowTasksApp/controllers/TopicController.js",
                "~/Scripts/flowTasksApp/controllers/UserController.js",
                "~/Scripts/flowTasksApp/controllers/TaskController.js",
                "~/Scripts/flowTasksApp/controllers/HolidayController.js",
                "~/Scripts/flowTasksApp/controllers/DashboardController.js",
                "~/Scripts/flowTasksApp/controllers/SketchController.js",
                "~/Scripts/flowTasksApp/controllers/ReportController.js"));

            // Added By New Ocean
            // Report scripts
            bundles.Add(new ScriptBundle("~/bundles/report").Include(
                "~/Scripts/jquery.multiselect.min.js"
                ));

            // Report css
            bundles.Add(new StyleBundle("~/Content/reportcss").Include(
                "~/Content/themes/base/jquery.multiselect.css"
                ));

            // Sketch Scripts
            bundles.Add(new ScriptBundle("~/bundles/sketchjs").Include(
                "~/Scripts/Sketch/jquery-ui.touch-punch.js",
                "~/Scripts/Sketch/sketchAngular.js",
                "~/Scripts/Sketch/raphael.js",
                "~/Scripts/Sketch/raphael.export.js",
                "~/Scripts/Sketch/raphael.json.js",
                "~/Scripts/Sketch/objectController.js",
                "~/Scripts/Sketch/entity.js",
                "~/Scripts/Sketch/utils.js",
                "~/Scripts/Sketch/graphics.js",
                "~/Scripts/Sketch/process.js",
                "~/Scripts/Sketch/jquery.jscrollpane.js",
                "~/Scripts/Sketch/jquery.mousewheel.js",
                "~/Scripts/Sketch/jquery.flexbox.js",
                "~/Scripts/Sketch/xml2json.js",
                "~/Scripts/Sketch/json2xml.js"
                ));

            // Sketch css
            bundles.Add(new StyleBundle("~/Content/sketchcss").Include(
                "~/Content/themes/jquery.jscrollpane.css",
                "~/Content/themes/jquery.flexbox.css"
                ));
        }
    }
}