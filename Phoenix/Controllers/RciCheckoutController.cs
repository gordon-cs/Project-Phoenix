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
            // Preliminary check to figure out which method to call
            var isIndividualRci = checkoutService.IsIndividualRci(id);

            if (!isIndividualRci) // A common area rci
            {
                ViewBag.commonRooms = checkoutService.GetCommonRooms(id);
                var rci = checkoutService.GetCommonAreaRciByID(id);
                return View("CommonArea", rci);
            }
            else // An individual room
            {
                var rci = checkoutService.GetIndividualRoomRciByID(id);
                return View("IndividualRoom", rci);
            }
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
        /// Return the html view where residents can sign their common area rci
        /// </summary>
        [HttpGet]
        public ActionResult CommonAreaSignature(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            var rci = checkoutService.GetCommonAreaRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the common area signatures
        /// </summary>
        [HttpPost]
        public void CommonAreaSignature()
        {

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

            var rci = checkoutService.GetIndividualRoomRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the resident's signature.
        /// </summary>
        [HttpPost]
        public ActionResult ResidentSignature(int id, string signature)
        {
            var role = (string)TempData["role"];

            var rci = checkoutService.GetIndividualRoomRciByID(id);
            if(rci.CheckoutSigRes != null) // Already signed
            {
                if(role == "Resident")
                {
                    return RedirectToAction(controllerName:"Dashboard", actionName:"Index");
                }
                else
                {
                    return RedirectToAction("RASignatureIndividual", new { id = id });
                }
                
            }

            // Not yet signed.
            var signatureMatch = (rci.FirstName.ToLower() + " " + rci.LastName.ToLower()).Equals(signature.ToLower());
            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                return View(rci);
            }

            checkoutService.CheckoutResidentSignRci(rci);

            if (role == "Resident")
            {
                return RedirectToAction(controllerName: "Dashboard", actionName: "Index");
            }
            else
            {
                return RedirectToAction("RASignatureIndividual", new { id = id });
            }
        }

        /// <summary>
        /// Return the html view where an RA can sign to checkout a resident.
        /// </summary>
        [HttpGet]
        public ActionResult RASignatureIndividual(int id)
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
            var rci = checkoutService.GetIndividualRoomRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the RA's signature
        /// </summary>
        [HttpPost]
        public ActionResult RASignatureIndividual(int id, string signature, DateTime date, bool improperCheckout = false, bool lostKey = false, decimal lostKeyFine = 0.00M)
        {
            var role = (string)TempData["role"];

            var rci = checkoutService.GetIndividualRoomRciByID(id);
            if(rci.CheckoutSigRA != null) // Already signed
            {
                if(role == "RD" || role == "ADMIN")
                {
                    return RedirectToAction("RDSignatureIndividual", new { id = id });
                }
                else
                {
                    return RedirectToAction(controllerName: "Dashboard", actionName: "Index");
                }
            }

            var signatureMatch = (((string)TempData["user"]).ToLower()).Equals(signature.ToLower());
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
            checkoutService.CheckoutRASignRci(rci,(string)TempData["user"], (string)TempData["id"]);

            if (role == "RD" || role == "ADMIN")
            {
                return RedirectToAction("RDSignatureIndividual", new { id = id });
            }
            else
            {
                return RedirectToAction(controllerName: "Dashboard", actionName: "Index");
            }
        }

        /// <summary>
        /// Return the html view where an RD can sign to checkout a resident.
        /// </summary>
        [HttpGet]
        public ActionResult RDSignatureIndividual(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (role.Equals("Resident"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (role.Equals("RA"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var rdName = (string)TempData["user"];
            ViewBag.ExpectedSignature = rdName;
            var rci = checkoutService.GetIndividualRoomRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the RD's signature
        /// </summary>
        [HttpPost]
        public ActionResult RDSignatureIndividual(int id, string signature, DateTime date, bool improperCheckout = false, bool lostKey = false, decimal lostKeyFine = 0.00M)
        {
            var rci = checkoutService.GetIndividualRoomRciByID(id);
            if (rci.CheckoutSigRD != null) // Already signed
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            var signatureMatch = (((string)TempData["user"]).ToLower()).Equals(signature.ToLower());
            if (!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ExpectedSignature = (string)TempData["user"];
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                return View(rci);
            }

            if (improperCheckout)
            {
                checkoutService.SetImproperCheckout(id);
            }
            if (lostKey)
            {
                checkoutService.SetLostKeyFine(id, lostKeyFine);
            }
            checkoutService.CheckoutRDSignRci(rci,(string)TempData["user"],(string)TempData["id"]);
            return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }
    }
}