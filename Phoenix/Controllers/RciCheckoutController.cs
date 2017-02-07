using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class RciCheckoutController : Controller
    {
        private RciCheckoutService checkoutService;

        public RciCheckoutController()
        {
            checkoutService = new RciCheckoutService();
        }

        // GET: RCICheckout
        /// <summary>
        /// Dislay the checkout view for the specified rci
        /// </summary>
        /// <param name="id">RCI identifier</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var rci = checkoutService.GetRciByID(id);

            return View(rci);
        }

        public void SaveRci(RciFinesForm rci)
        {
            checkoutService.AddFines(rci.NewFines, rci.GordonID);
            checkoutService.RemoveFines(rci.FinesToDelete);

            return;
        }

        public ActionResult ResidentSignature(int id)
        {
            var rci = checkoutService.GetRciByID(id);
            return View(rci);
        }

        public ActionResult RASignature(int id)
        {
            var rci = checkoutService.GetRciByID(id);
            return View(rci);
        }
    }
}