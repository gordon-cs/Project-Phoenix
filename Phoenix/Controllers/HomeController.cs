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
            var room = new Room { RoomID = "1", Capacity = 2 };
            _context.Room.Add(room);
            _context.SaveChanges();

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