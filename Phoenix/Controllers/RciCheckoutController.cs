using Phoenix.Filters;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RciCheckoutController : Controller
    {
        private IRciCheckoutService CheckoutService { get; set; }

        private ILoginService LoginService { get; set; }

        public RciCheckoutController(ILoginService loginService, IRciCheckoutService checkoutService)
        {
            this.CheckoutService = checkoutService;

            this.LoginService = loginService;
        }

        // GET: RCICheckout
        /// <summary>
        /// Dislay the checkout view for the specified rci
        /// </summary>
        /// <param name="id">RCI identifier</param>
        /// <returns></returns>
        [ResLifeStaff]
        public ActionResult Index(int id)
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];
            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var temp = this.CheckoutService.GetBareRciByID(id);

            // Preliminary check to figure out which method to call
            var isIndividualRci = temp.IsIndividualRci();

            if (!isIndividualRci) // A common area rci
            {
                ViewBag.commonRooms = this.CheckoutService.GetCommonRooms(id);
                var rci = this.CheckoutService.GetCommonAreaRciByID(id);
                if (rci.CheckoutSigRD != null)
                {
                    return RedirectToAction("RciReview", new { id = id } );
                }
                return View("CommonArea", rci);
            }
            else // An individual room
            {
                var rci = this.CheckoutService.GetIndividualRoomRciByID(id);
                if (rci.CheckoutSigRD != null)
                {
                    return RedirectToAction("RciReview", new { id = id });
                }
                return View("IndividualRoom", rci);
            }
        }

        /// <summary>
        ///  Return the checkout review view
        /// </summary>
        public ActionResult RciReview(int id)
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];
            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var temp = this.CheckoutService.GetBareRciByID(id);

            // Check to figure out if this RCI can be viewed by the logged in user
            var isViewable = temp.isViewableBy((string)TempData["id"], role, (string)TempData["currentRoom"], (string)TempData["currentBuilding"]);
            if(!isViewable)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            // Check to figure out which method to call
            var isIndividualRci = temp.IsIndividualRci();

            if (!isIndividualRci) // A common area rci
            {
                ViewBag.commonRooms = this.CheckoutService.GetCommonRooms(id);
                var rci = this.CheckoutService.GetCommonAreaRciByID(id);
                return View("RciReviewCommonArea", rci);
            }
            else // An individual room
            {
                var rci = this.CheckoutService.GetIndividualRoomRciByID(id);
                return View("RciReviewIndividualRoom", rci);
            }
        }
        /// <summary>
        /// Add a new fine and return its id
        /// </summary>
        [ResLifeStaff]
        public int AddFine(RciNewFineViewModel fine)
        {
            var fineID = this.CheckoutService.AddFine(fine);
            return fineID;
        }

        /// <summary>
        /// Delete an existing fine and return a status code
        /// </summary>
        [ResLifeStaff]
        public ActionResult RemoveFine(int fineID)
        {
            this.CheckoutService.RemoveFine(fineID);
            return new HttpStatusCodeResult(200, "OK");
        }

        /// <summary>
        /// Return the html view where residents can sign their common area rci
        /// </summary>
        [HttpGet]
        [ResLifeStaff]
        public ActionResult CommonAreaSignature(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            var rci = this.CheckoutService.GetCommonAreaRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the common area signatures.
        /// Once everyone has signed, the CheckoutSigRes column is filled.
        /// </summary>
        [HttpPost]
        [ResLifeStaff]
        public ActionResult CommonAreaSignature(int id, string[] signature)
        {
            var  signatures = new List<string>(signature);
            signatures.RemoveAll( x => x == ""); // Remove empty strings.

            for (var i = 0; i < signatures.Count; i++)
            {
                signatures[i] = signatures[i].ToLower();
            }
            

            var rci = this.CheckoutService.GetCommonAreaRciByID(id);

            if (rci.EveryoneHasSigned()) // Already signed
            {
                    return RedirectToAction("RASignature", new { id = id });
            }

            // Not yet signed
            foreach (var member in rci.CommonAreaMember)
            {
                var expectedSignature = member.FirstName.ToLower() + " " + member.LastName.ToLower();
                if(signatures.Contains(expectedSignature))
                {
                    signatures.Remove(expectedSignature);

                    if(!member.HasSignedCommonAreaRci)
                    {
                        this.CheckoutService.CheckoutCommonAreaMemberSignRci(id, member.GordonID);
                    }
                }
            }
            
            // There were some signatures that were submitted, but did not match
            if(signatures.Count > 0) 
            {
                var errorMessages = new List<string>();
                foreach(var sig in signatures)
                {
                    errorMessages.Add("The name " + sig + " does not match.");
                }
                ViewBag.ErrorMessage = errorMessages;
                rci = this.CheckoutService.GetCommonAreaRciByID(id); // Reload the rci to reflect those who have already signed.
                return View(rci);
            }

            rci = this.CheckoutService.GetCommonAreaRciByID(id); // reload rci from db to see if everyone has now signed.
            // If at the end of the loop, the boolean is still true, then everyone has signed.
            if(rci.EveryoneHasSigned())
            {
                this.CheckoutService.CheckoutResidentSignRci(id); // This is set once everybody has signed
                return RedirectToAction("RASignature", new { id = id });
            }
            else
            {
                return RedirectToAction(controllerName: "Dashboard", actionName: "Index");
            }

        }


        /// <summary>
        /// Return the html view where a resident can sign to checkout
        /// </summary>
        [HttpGet]
        [ResLifeStaff]
        public ActionResult ResidentSignature(int id)
        {
            var rci = this.CheckoutService.GetIndividualRoomRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the resident's signature.
        /// </summary>
        [HttpPost]
        [ResLifeStaff]
        public ActionResult ResidentSignature(int id, string signature)
        {
            var rci = this.CheckoutService.GetIndividualRoomRciByID(id);

            if(rci.CheckoutSigRes != null) // Already signed
            {
                    return RedirectToAction("RASignature", new { id = id });
            }

            // Not yet signed.
            var signatureMatch = (rci.FirstName.ToLower() + " " + rci.LastName.ToLower()).Equals(signature.ToLower());
            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                return View(rci);
            }

            this.CheckoutService.CheckoutResidentSignRci(rci.RciID);
            return RedirectToAction("RASignature", new { id = id });
        }


        /// <summary>
        /// Return the html view where an RA can sign to checkout a resident.
        /// </summary>
        [HttpGet]
        [ResLifeStaff]
        public ActionResult RASignature(int id)
        {
            var raName = (string)TempData["user"];
            ViewBag.ExpectedSignature = raName;
            var rci = this.CheckoutService.GetGenericCheckoutRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the RA's signature
        /// </summary>
        [HttpPost]
        [ResLifeStaff]
        public ActionResult RASignature(int id, string signature)
        {
            var role = (string)TempData["role"];

            var rci = this.CheckoutService.GetGenericCheckoutRciByID(id);
            if(rci.CheckoutSigRA != null) // Already signed
            {
                return RedirectToAction("RDSignature", new { id = id });
            }

            var signatureMatch = (((string)TempData["user"]).ToLower()).Equals(signature.ToLower());
            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ExpectedSignature = (string)TempData["user"];
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                return View(rci);
            }

            this.CheckoutService.CheckoutRASignRci(rci.RciID, (string)TempData["id"]);
            return RedirectToAction("RDSignature", new { id = id });
        }

        /// <summary>
        /// Return the html view where an RD can sign to checkout a resident.
        /// </summary>
        [HttpGet]
        [RD]
        public ActionResult RDSignature(int id)
        {
            var name = (string)TempData["user"];
            ViewBag.RDName = name;

            var userName = (string)TempData["login_username"];
            ViewBag.ExpectedUsername = userName;
            var rci = this.CheckoutService.GetGenericCheckoutRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the RD's signature
        /// </summary>
        [HttpPost]
        [RD]
        public ActionResult RDSignature(int id, string password, string username,  string phoneNumber, List<string> workRequest)
        {
            var rci = this.CheckoutService.GetGenericCheckoutRciByID(id);
            if (rci.CheckoutSigRD != null) // Already signed
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            if (username.EndsWith("@gordon.edu"))
            {
                username = username.Remove(username.IndexOf('@'));
            }

            var isValidLogin = this.LoginService.IsValidUser(username, password, this.LoginService.ConnectToADServer());
            if (!isValidLogin) // If this is not a valid user.
            {
                ViewBag.RDName = (string)TempData["user"]; ;
                ViewBag.ExpectedUsername = username;
                ViewBag.ErrorMessage = "Oh dear, it seems that username or password is invalid.";
                ViewBag.WorkRequests = workRequest == null ? new List<string>() : workRequest ; // Send back the list of work requests they wanted.
                return View(rci);
            }

            // If they are submitting work requests, check to see that they provided a phone number
            if(workRequest != null && workRequest.Count > 0 && (phoneNumber == null || phoneNumber.Trim().Equals("")) )
            {
                ViewBag.RDName = (string)TempData["user"]; ;
                ViewBag.ExpectedUsername = username;
                ViewBag.ErrorMessage = "You are submitting work requests, but have not provided a phone number. Please enter a phone number.";
                ViewBag.WorkRequests = workRequest; // Send back the list of work requests they wanted.
                return View(rci);
            }

            var rciSigned = false;

            rciSigned = this.CheckoutService.CheckoutRDSignRci(rci.RciID, (string)TempData["id"]);

            if (rciSigned) // Only do the rest if the rci was successfully signed.
            {
                 this.CheckoutService.SendFineEmail(id, username + "@gordon.edu", password);
                if(workRequest != null)
                {
                    this.CheckoutService.WorkRequestDamages(workRequest, username, password, id, phoneNumber);
                }
            }
            else
            {
                ViewBag.RDName = (string)TempData["user"];
                ViewBag.ExpectedUsername = username;
                ViewBag.WorkRequests = workRequest == null ? new List<string>() : workRequest;
                ViewBag.ErrorMessage = "There was a problem signing this rci. No emails or work requests have been sent. Please try again.";

                return View(rci);
            }

            return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }
    }
}