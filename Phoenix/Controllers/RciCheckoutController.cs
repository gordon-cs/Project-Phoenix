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
            var rci = this.CheckoutService.GetRciById(id);

            var isCommonAreaRci = rci.GordonId == null;

            if (isCommonAreaRci) // A common area rci
            {
                if (rci.RdCheckoutDate == null)
                {
                    return View("CommonArea", rci);
                }
                else
                {
                    return RedirectToAction("RciReview", new { id } );
                }
            }
            else // An individual room
            {
                if (rci.RdCheckoutDate == null)
                {
                    return View("IndividualRoom", rci);
                }
                else
                {
                    return RedirectToAction("RciReview", new { id });
                }
                
            }
        }

        /// <summary>
        ///  Return the checkout review view
        /// </summary>
        public ActionResult RciReview(int id)
        {
            var rci = this.CheckoutService.GetRciById(id);

            var role = (string)TempData["role"];

            // Check to figure out if this RCI can be viewed by the logged in user
            var isViewable = rci.isViewableBy((string)TempData["id"], role, (string)TempData["currentRoom"], (string)TempData["currentBuilding"]);

            if(!isViewable)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            var isCommonAreaRci = rci.GordonId == null;

            if (isCommonAreaRci) // A common area rci
            {
                return View("RciReviewCommonArea", rci);
            }
            else // An individual room
            {
                return View("RciReviewIndividualRoom", rci);
            }
        }

        /// <summary>
        /// Add a new fine and return its id
        /// </summary>
        [ResLifeStaff]
        public int AddFine(RciNewFineViewModel fine)
        {
            return this.CheckoutService.AddFine(fine);
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
            var rci = this.CheckoutService.GetRciById(id);

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
            var signatures = new List<string>(signature);

            signatures.RemoveAll(x => x == ""); // Remove empty strings.

            for (var i = 0; i < signatures.Count; i++)
            {
                signatures[i] = signatures[i].ToLower().Trim();
            }

            var rci = this.CheckoutService.GetRciById(id);

            bool everyoneHasSigned = rci.CommonAreaMembers.TrueForAll(x => x.CheckoutDate != null);

            if (everyoneHasSigned) // Already signed
            {
                return RedirectToAction("RASignature", new { id });
            }

            // Not yet signed
            foreach (var member in rci.CommonAreaMembers)
            {
                var expectedSignature = $"{member.FirstName.Trim()} {member.LastName.Trim()}".ToLower().Trim();

                if (signatures.Contains(expectedSignature))
                {
                    signatures.Remove(expectedSignature);

                    if (member.CheckoutDate == null)
                    {
                        this.CheckoutService.CheckoutCommonAreaMemberSignRci(id, member.GordonId);
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

                rci = this.CheckoutService.GetRciById(id); // Reload the rci to reflect those who have already signed.

                return View(rci);
            }

            rci = this.CheckoutService.GetRciById(id); // reload rci from db to see if everyone has now signed.

            everyoneHasSigned = rci.CommonAreaMembers.TrueForAll(x => x.CheckoutDate != null);

            if (everyoneHasSigned)
            {
                this.CheckoutService.CheckoutResidentSignRci(id); // This is set once everybody has signed

                return RedirectToAction("RASignature", new { id });
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
            var rci = this.CheckoutService.GetRciById(id);

            return View(rci);
        }

        /// <summary>
        /// Verify the resident's signature.
        /// </summary>
        [HttpPost]
        [ResLifeStaff]
        public ActionResult ResidentSignature(int id, string signature)
        {
            var rci = this.CheckoutService.GetRciById(id);

            if(rci.ResidentCheckoutDate != null) // Already signed
            {
                return RedirectToAction("RASignature", new { id });
            }

            var residentName = $"{rci.FirstName.Trim()} {rci.LastName.Trim()}".ToLower().Trim();

            var signatureMatch = residentName.Equals(signature.ToLower());

            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";
                
                return View(rci);
            }

            this.CheckoutService.CheckoutResidentSignRci(rci.RciId);

            return RedirectToAction("RASignature", new { id });
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

            var rci = this.CheckoutService.GetRciById(id);

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

            var rci = this.CheckoutService.GetRciById(id);

            if(rci.RaCheckoutDate != null) // Already signed
            {
                return RedirectToAction("RDSignature", new { id });
            }

            var signatureMatch = (((string)TempData["user"]).ToLower().Trim()).Equals(signature.ToLower().Trim());

            if(!signatureMatch) // Signature provided doesn't match
            {
                ViewBag.ExpectedSignature = (string)TempData["user"];

                ViewBag.ErrorMessage = "The Signatures did not match! The signature should match the name indicated.";

                return View(rci);
            }

            this.CheckoutService.CheckoutRASignRci(rci.RciId, (string)TempData["id"]);

            return RedirectToAction("RDSignature", new { id });
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

            var rci = this.CheckoutService.GetRciById(id);

            return View(rci);
        }

        /// <summary>
        /// Verify the RD's signature
        /// </summary>
        [HttpPost]
        [RD]
        public ActionResult RDSignature(int id, string password, string username,  string phoneNumber, List<string> workRequest)
        {
            var rci = this.CheckoutService.GetRciById(id);

            if (rci.RdCheckoutDate != null) // Already signed
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            if (username.EndsWith("@gordon.edu"))
            {
                username = username.Remove(username.IndexOf('@'));
            }

            var adServer = this.LoginService.ConnectToADServer();

            var isValidLogin = this.LoginService.IsValidUser(username, password, adServer);

            if (!isValidLogin) // If this is not a valid user.
            {
                ViewBag.RDName = (string)TempData["user"];

                ViewBag.ExpectedUsername = username;

                ViewBag.ErrorMessage = "Oh dear, it seems that username or password is invalid.";

                ViewBag.WorkRequests = workRequest ?? new List<string>(); // Send back the list of work requests they wanted.

                return View(rci);
            }

            var gordonId = this.LoginService.FindUser(username, adServer).EmployeeId;

            // If they are submitting work requests, check to see that they provided a phone number
            if(workRequest != null && workRequest.Count > 0 && (phoneNumber == null || phoneNumber.Trim().Equals("")) )
            {
                ViewBag.RDName = (string)TempData["user"];

                ViewBag.ExpectedUsername = username;

                ViewBag.ErrorMessage = "You are submitting work requests, but have not provided a phone number. Please enter a phone number.";

                ViewBag.WorkRequests = workRequest; // Send back the list of work requests they wanted.

                return View(rci);
            }

            this.CheckoutService.CheckoutRDSignRci(rci.RciId, (string)TempData["id"]);

            this.CheckoutService.SendFineEmail(id, username + "@gordon.edu", password);

            this.CheckoutService.WorkRequestDamages(workRequest, username, password, gordonId, id, phoneNumber);

            return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }
    }
}