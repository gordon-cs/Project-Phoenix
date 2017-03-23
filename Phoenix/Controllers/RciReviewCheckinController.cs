using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class RciReviewCheckinController : Controller
    {
        private RciReviewCheckinService reviewService;

        public RciReviewCheckinController()
        {
            reviewService = new RciReviewCheckinService();
        }
        public ActionResult Index(int id)
        {
            var rci = reviewService.GetRciByID(id);
            return View(rci);
        }
    }
}