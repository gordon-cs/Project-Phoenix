using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class PlaceholderController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // I am not sure if TempData is going to be the best way to persist 
            // the user's name while they are on the site.
            // It works great for redirects, but I am not sure how it will work for navigation within the site
            if (TempData["Name"] != null)
            {
                ViewBag.Name = TempData["Name"];
            }
            return View();
        }
    }
}