using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class RciReviewCheckoutController : Controller
    {
        private RciReviewCheckoutService reviewService;

        public RciReviewCheckoutController()
        {
            reviewService = new RciReviewCheckoutService();
        }

        // GET: RciReviewCheckout
        public ActionResult Index(int id)
        {
            var rci = reviewService.GetRciByID(id);
            return View(rci);
        }
    }
}