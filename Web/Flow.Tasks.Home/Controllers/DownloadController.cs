using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flow.Tasks.Home.Controllers
{
    public class DownloadController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "FlowTasks - Download";

            return View();
        }
    }
}
