using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OnlineHelpDesk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineHelpDesk.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult ViewProfile()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            //var userId= User.Identity.GetUserId();
            var profile = new ProfileViewModell
            {
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,

            };
            return View(profile);
        }
        public ActionResult NotificationDetail(int? id)
        {
            //var user = UserManager.FindById(User.Identity.GetUserId());
            var db = new ApplicationUser();
            var noti = db.Notifications.Where(i => i.Id == id.Value);
            //if (id != null)
            //{
            //    Notification notification = db.Notifications.Where(i => i.Id == id.Value).Single();
            //    return View(notification);
            //}
            return View(noti);
        }

            public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ListRequest()
        {
            return View();
        }

        public ActionResult CreateNewRequest()
        {
            return View();
        }
    }
}