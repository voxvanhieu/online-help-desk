using Microsoft.AspNet.Identity;
<<<<<<< HEAD
using Microsoft.AspNet.Identity.Owin;
=======
using Microsoft.AspNet.Identity.EntityFramework;
>>>>>>> dev
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
<<<<<<< HEAD
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
=======
        ApplicationDbContext context = new ApplicationDbContext();
        public ActionResult Index()
>>>>>>> dev
        {
            var userId = User.Identity.GetUserId();
            List<Notification> notifications = (from n in context.Notifications
                                                where n.UserId == userId
                                                select n).ToList();
            int requestCount = context.Requests.Count();
            int userCount = context.Users.Count();
            int facilityCount = context.Facilities.Count();
            int equipmentCount = context.Equipments.Count();
            return View(new HomeViewModel() { 
                Notifications = notifications, 
                RequestViewModels = null,
                Requests = requestCount,
                Users = userCount,
                Facilities = facilityCount,
                Equipments = equipmentCount
            });
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
            string userId = User.Identity.GetUserId();
            List <Notification> notifications = (from n in context.Notifications
                                                where n.UserId == userId
                                                select n).ToList();
            var requestRecords = from r in context.Requests
                                join e in context.Equipments on r.EquipmentId equals e.Id into tb1
                                from e in tb1.ToList()
                                join f in context.Facilities on e.FacilityId equals f.Id
                                join et in context.EquipmentTypes on e.ArtifactId equals et.id
                                join rs in context.RequestStatus on r.RequestStatusId equals rs.Id into tb2
                                from rs in tb2.ToList()
                                join st in context.StatusTypes on rs.StatusTypeId equals st.Id
                                join rt in context.RequestTypes on r.RequestTypeId equals rt.Id
                                join u in context.Users on r.PetitionerId equals u.Id
                                select new RequestViewModel
                                {
                                    Id = r.Id,
                                    Petitioner = u.UserName,
                                    Equipment = et.artifact_name,
                                    Facility = f.Name,
                                    RequestType = rt.TypeName,
                                    RequestMessage = r.Message,
                                    CreatedTime = rs.TimeCreated
                                };

            List<RequestViewModel> requestViewModels = requestRecords.ToList();

            return View(new HomeViewModel() { Notifications = notifications, RequestViewModels = requestViewModels });
        }

        public ActionResult CreateNewRequest()
        {
            var userId = User.Identity.GetUserId();
            List<Notification> notifications = (from n in context.Notifications
                                                where n.UserId == userId
                                                select n).ToList();
            return View(new HomeViewModel() { Notifications = notifications, RequestViewModels = null });
        }
    }
}