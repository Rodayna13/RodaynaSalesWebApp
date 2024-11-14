using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesWebApp.Controllers
{
    public class LanguageController : Controller
    {
        public ActionResult ChangeLanguage(string lang)
        {
            // Validate input language
            lang = lang.ToLower() == "ar" ? "ar" : "en";

            // Store language preference in cookie
            HttpCookie cookie = new HttpCookie("Language", lang);
            cookie.Expires = DateTime.Now.AddYears(1); // Cookie expiration
            Response.Cookies.Add(cookie);

            // Redirect back to the previous page
            string returnUrl = Request.UrlReferrer?.ToString();
            return Redirect(returnUrl ?? "/");
        }
    }
}
