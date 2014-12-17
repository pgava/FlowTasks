using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flow.Tasks.Home.Controllers
{
    public class DocController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "FlowTasks";

            return View();
        }
    }
}
