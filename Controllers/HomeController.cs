using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Username"] == null || string.IsNullOrEmpty(Session["SessionId"]?.ToString()))
            {
                return RedirectToAction("Register", "Account");
            }

            // Check if session is valid
            var context = new AppDbContext();
            var userRepository = new UserRepository(context);
            var user = userRepository.GetByUsername(Session["Username"].ToString());

            if (user == null || user.SessionId != Session["SessionId"].ToString() || user.LastLogin.AddHours(3) <= DateTime.Now)
            {
                return RedirectToAction("Register", "Account");
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}