using Core.Interfaces;
using Core.UseCases;

using SalesWebApp.Models;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace SalesWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginUseCase _loginUseCase;
        private readonly RegisterUseCase _registerUseCase;
        private readonly IUserRepository _userRepository;

        public AccountController(LoginUseCase loginUseCase, RegisterUseCase registerUseCase, IUserRepository userRepository)
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["Username"] != null)
            {
                // Check if session is still valid
                var sessionId = Session["SessionId"]?.ToString();
                if (!string.IsNullOrEmpty(sessionId))
                {
                    var user = _userRepository.GetByUsername(Session["Username"].ToString());
                    if (user != null && user.SessionId == sessionId && user.LastLogin.AddHours(3) > DateTime.Now)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_loginUseCase.Execute(model.Username, model.Password, out var sessionId))
                {
                    Session["Username"] = model.Username;
                    Session["SessionId"] = sessionId;

                    // Set session timeout 3 hours
                    Session.Timeout = 180;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid username or password";
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ViewBag.ErrorMessage = "Passwords do not match.";
                    return View(model);
                }

                if (_registerUseCase.Execute(model.Username, model.Password))
                {
                    return RedirectToAction("Login");
                }
                ViewBag.ErrorMessage = "This user already exists,try login.";
            }
            // If  not valid, return validation errors
            return View(model);
        }

        public ActionResult Logout()
        {
            var sessionId = Session["SessionId"]?.ToString();
            if (!string.IsNullOrEmpty(sessionId))
            {
                _userRepository.ClearSession(sessionId);
            }

            Session.Clear();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }
    }
}
