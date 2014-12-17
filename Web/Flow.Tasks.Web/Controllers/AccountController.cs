using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Flow.Tasks.Web.Models;
using Flow.Tasks.View;
using Flow.Tasks.Contract;
using Flow.Docs.Contract.Interface;
using Flow.Users.Contract;
using Flow.Users.Contract.Message;

namespace Flow.Tasks.Web.Controllers
{

    /// <summary>
    /// Account Controller
    /// </summary>
    [HandleError]
    public class AccountController : BaseController
    {
        public static readonly string AdminRole = "Admin";

        public AccountController(IFlowUsersService usersService, IFlowTasksService tasksService, IFlowDocsDocument docsDocument) :
            base(usersService, tasksService, docsDocument) { }

        /// <summary>
        /// Log On Action
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// Log On Action
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="returnUrl">ReturnUrl</param>
        /// <returns>ActionResult</returns>
#if CODE_PRE_NEW_UI        
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var data = UsersService.AuthenticateUser(new AuthenticateUserRequest { User = model.UserName, Password = model.Password });
                if (data.IsAuthenticated)
                {
                    return AuthorizeUserToDomains(data, returnUrl);
                    //return ProcessUserData(data, returnUrl);
                }
                ModelState.AddModelError("", @"The user name or password provided is incorrect.");
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }
#endif
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var data = UsersService.AuthenticateUser(new AuthenticateUserRequest { User = model.UserName, Password = model.Password });
                if (data.IsAuthenticated)
                {
                    return AuthorizeUserToDomains(data, "", model.RememberMe);
                }
                ModelState.AddModelError("", @"The user name or password provided is incorrect.");                
            }

            // If we got this far, something failed, redisplay form
            return Json(false);
        }

        /// <summary>
        /// Log Off
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
