using Phoenix.Filters;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using System;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
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
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];
            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var rci = checkoutService.GetRciByID(id);

            return View(rci);
        }

        /// <summary>
        /// Similar to the SaveRci Method for RciInputController
        /// </summary>
        public void SaveRci(RciFinesForm rci)
        {
            checkoutService.AddFines(rci.NewFines, rci.GordonID);
            checkoutService.RemoveFines(rci.FinesToDelete);

            return;
        }

        /// <summary>
        /// Return the html view where a resident can sign to checkout
        /// </summary>
        [HttpGet]
        public ActionResult ResidentSignature(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            var rci = checkoutService.GetRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the resident's signature.
        /// </summary>
        [HttpPost]
        public ActionResult ResidentSignature(int id, string signature, DateTime date)
        {
            var rci = checkoutService.GetRciByID(id);
            if(rci.CheckoutSigRes != null) // Already signed
            {
                return RedirectToAction("RASignature", new { id = id });
            }

            var signatureMatch = (rci.FirstName + " " + rci.LastName).Equals(signature);
            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                return View(rci);
            }

            checkoutService.CheckoutResidentSignRci(rci);
            return RedirectToAction("RASignature", new { id = id });
        }

        /// <summary>
        /// Return the html view where an RA can sign to checkout a resident.
        /// </summary>
        [HttpGet]
        public ActionResult RASignature(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (role.Equals("Resident"))
            {
                return RedirectToAction("ResidentSignature", new { id = id });
            }

            var raName = (string)TempData["user"];
            ViewBag.ExpectedSignature = raName;
            var rci = checkoutService.GetRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the RA's signature
        /// </summary>
        [HttpPost]
        public ActionResult RASignature(int id, string signature, DateTime date, bool improperCheckout = false, bool lostKey = false, decimal lostKeyFine = 0.00M)
        {
            var rci = checkoutService.GetRciByID(id);
            if(rci.CheckoutSigRA != null) // Already signed
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            var signatureMatch = ((string)TempData["user"]).Equals(signature);
            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ExpectedSignature = (string)TempData["user"];
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                return View(rci);
            }

            if(improperCheckout)
            {
                checkoutService.SetImproperCheckout(id);
            }
            if(lostKey)
            {
                checkoutService.SetLostKeyFine(id, lostKeyFine);
            }
            checkoutService.CheckoutRASignRci(rci);
            return RedirectToAction(actionName:"Index", controllerName:"Dashboard");
        }
    }
}