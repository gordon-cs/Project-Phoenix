using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Phoenix.Models;
using Phoenix.Models.PreExistingViews;

namespace Phoenix.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var _context = new RCIContext();
            var accounts = _context.Account.ToList();

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
    }
}