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
    public class RCICheckoutController : Controller
    {
        private RCICheckoutService checkoutService;

        public RCICheckoutController()
        {
            checkoutService = new RCICheckoutService();
        }

        // GET: RCICheckout
        /// <summary>
        /// Dislay the checkout view for the specified rci
        /// </summary>
        /// <param name="id">RCI identifier</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var rci = checkoutService.GetRCIByID(id);

            return View(rci);
        }

        public void SaveRCI(RCIFinesForm rci)
        {
            checkoutService.AddFines(rci.newFines, rci.gordonID);
            checkoutService.RemoveFines(rci.finesToDelete);

            return;
        }

        public ActionResult ResidentSignature(int id)
        {
            var rci = checkoutService.GetRCIByID(id);
            return View(rci);
        }

        public ActionResult RASignature(int id)
        {
            var rci = checkoutService.GetRCIByID(id);
            return View(rci);
        }
    }
}