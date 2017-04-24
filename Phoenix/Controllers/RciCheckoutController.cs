﻿using Phoenix.Filters;
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
        private RciCheckoutService checkoutService;
        private RoomComponentService componentService;
        private LoginService loginService;

        public RciCheckoutController()
        {
            checkoutService = new RciCheckoutService();
            loginService = new LoginService();
            componentService = new RoomComponentService();
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

            var temp = checkoutService.GetBareRciByID(id);

            // Preliminary check to figure out which method to call
            var isIndividualRci = temp.IsIndividualRci();

            if (!isIndividualRci) // A common area rci
            {
                ViewBag.commonRooms = checkoutService.GetCommonRooms(id);
                var rci = checkoutService.GetCommonAreaRciByID(id);
                ViewBag.CostDictionary = componentService.GetCostDictionary("common", rci.BuildingCode);
                if (rci.CheckoutSigRD != null)
                {
                    return RedirectToAction("RciReview");
                }
                return View("CommonArea", rci);
            }
            else // An individual room
            {
                var rci = checkoutService.GetIndividualRoomRciByID(id);
                ViewBag.CostDictionary = componentService.GetCostDictionary("individual", rci.BuildingCode);
                if (rci.CheckoutSigRD != null)
                {
                    return RedirectToAction("RciReview");
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

            var temp = checkoutService.GetBareRciByID(id);

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
                ViewBag.commonRooms = checkoutService.GetCommonRooms(id);
                var rci = checkoutService.GetCommonAreaRciByID(id);
                return View("RciReviewCommonArea", rci);
            }
            else // An individual room
            {
                var rci = checkoutService.GetIndividualRoomRciByID(id);
                return View("RciReviewIndividualRoom", rci);
            }
        }
        /// <summary>
        /// Add a new fine and return its id
        /// </summary>
        [ResLifeStaff]
        public int AddFine(RciNewFineViewModel fine)
        {
            var fineID = checkoutService.AddFine(fine);
            return fineID;
        }

        /// <summary>
        /// Delete an existing fine and return a status code
        /// </summary>
        [ResLifeStaff]
        public ActionResult RemoveFine(int fineID)
        {
            checkoutService.RemoveFine(fineID);
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

            var rci = checkoutService.GetCommonAreaRciByID(id);
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
            

            var rci = checkoutService.GetCommonAreaRciByID(id);

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
                        checkoutService.CheckoutCommonAreaMemberSignRci(id, member.GordonID);
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
                rci = checkoutService.GetCommonAreaRciByID(id); // Reload the rci to reflect those who have already signed.
                return View(rci);
            }

            rci = checkoutService.GetCommonAreaRciByID(id); // reload rci from db to see if everyone has now signed.
            // If at the end of the loop, the boolean is still true, then everyone has signed.
            if(rci.EveryoneHasSigned())
            {
                checkoutService.CheckoutResidentSignRci(id); // This is set once everybody has signed
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
            var rci = checkoutService.GetIndividualRoomRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the resident's signature.
        /// </summary>
        [HttpPost]
        [ResLifeStaff]
        public ActionResult ResidentSignature(int id, string signature)
        {
            var rci = checkoutService.GetIndividualRoomRciByID(id);

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

            checkoutService.CheckoutResidentSignRci(rci.RciID);
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
            var rci = checkoutService.GetGenericCheckoutRciByID(id);
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

            var rci = checkoutService.GetGenericCheckoutRciByID(id);
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

            checkoutService.CheckoutRASignRci(rci.RciID, (string)TempData["id"]);
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
            ViewBag.ExpextedSignature = name;

            var userName = (string)TempData["login_username"];
            ViewBag.ExpectedUsername = userName;
            var rci = checkoutService.GetGenericCheckoutRciByID(id);
            return View(rci);
        }

        /// <summary>
        /// Verify the RD's signature
        /// </summary>
        [HttpPost]
        [RD]
        public ActionResult RDSignature(int id, string password, string username)
        {

            var rci = checkoutService.GetGenericCheckoutRciByID(id);
            if (rci.CheckoutSigRD != null) // Already signed
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            var isValidLogin = loginService.IsValidUser(username, password, loginService.ConnectToADServer());
            if (!isValidLogin) // If this is not a valid user.
            {
                ViewBag.ExpectedUsername = username;
                ViewBag.ErrorMessage = "It looks like the credentials provided are invalid. Please try again.";
                return View(rci);
            }

            checkoutService.CheckoutRDSignRci(rci.RciID, (string)TempData["id"]);
            checkoutService.SendFineEmail(id, username + "@gordon.edu", password);
            return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }
    }
}