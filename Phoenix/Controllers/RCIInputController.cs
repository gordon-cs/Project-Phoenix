using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System;
using Phoenix.Services;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RCIInputController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private RCIContext db;
        private RCIInputService rciInputService;

        // This list is static so it will persist across actions.
        private static List<int> damagesToDelete = new List<int>();

        public RCIInputController()
        {
            Debug.WriteLine("Initialize RCIInput Controller");
            db = new Models.RCIContext();
            rciInputService = new RCIInputService();
        }

        public ActionResult Index(int id)
        {
            Debug.WriteLine("Reached Index Method for RCIInput Controller");

            // This is how we access items set in the filter.
            var gordon_id = (string)TempData["id"];

            //var rci = db.RCI.Where(m => m.RCIID == id).FirstOrDefault();
            var rci = rciInputService.GetRCI(id); 
            if (rci.gordonID == null) // A common area rci
            {
                ViewBag.ViewTitle = rci.buildingCode + rci.roomNumber + " Common Area";
            }
            else
            {
                var name = db.Account.Where(m => m.ID_NUM == rci.gordonID)
                    .Select(m => m.firstname + " " + m.lastname).FirstOrDefault();
                ViewBag.ViewTitle = rci.buildingCode + rci.roomNumber + " " + name;
            }
            
            return View(rci);
        }

        // Redirect to checkin signature page for certain roles.
        public ActionResult CheckinSig(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role.Equals("RD"))
            {
                return RedirectToAction("CheckinSigRD", new { id = id });
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("CheckinSigRA", new { id = id });
            }
            else
            {
                return RedirectToAction("CheckinSigRes", new { id = id });
            }
        }

        // GET: RCIInput/CheckinSigRes/1
        public ActionResult CheckinSigRes(int id)
        {
            var rci = rciInputService.GetRCI(id);
            ViewBag.Username = rciInputService.GetUsername(rci.gordonID);
            return View(rci);
        }

        // GET: RCIInput/CheckinSigRA/1
        public ActionResult CheckinSigRA(int id)
        {
            var rci = rciInputService.GetRCI(id);
            var gordonID = (string)TempData["id"];
            ViewBag.Username = rciInputService.GetUsername(gordonID);
            return View(rci);
        }

        // GET: RCIInput/CheckinSigRD/1
        public ActionResult CheckinSigRD(int id)
        {
            var rci = rciInputService.GetRCI(id);
            var gordonID = (string)TempData["id"];
            ViewBag.Username = rciInputService.GetUsername(gordonID);
            return View(rci);
        }

        // Save signatures for resident
        [HttpPost]
        public void SaveSigRes(string rciSig, string lacSig, int id)
        {
            rciSig = rciSig.ToLower().Trim();
            lacSig = lacSig.ToLower().Trim();
            var rci = db.RCI.Where(m => m.rciID == id).FirstOrDefault();
            var gordonID = (string)TempData["id"];
            var username = rciInputService.GetUsername(gordonID).ToLower().Trim();
            if (rciSig == username)
            {
                rci.checkinSigRes = DateTime.Today;
            }
            if (lacSig == username)
            {
                rci.lifeAndConductSigRes = DateTime.Today;
            }
            db.SaveChanges();
        }

        // Save signatures for RA
        [HttpPost]
        public void SaveSigRA(string rciSig, int id)
        {
            rciSig = rciSig.ToLower().Trim();
            var rci = db.RCI.Where(m => m.rciID == id).FirstOrDefault();
            var gordonID = (string)TempData["id"];
            var username = rciInputService.GetUsername(gordonID).ToLower().Trim();
            if (rciSig == username)
            {
                rci.checkinSigRA = DateTime.Today;
            }
            db.SaveChanges();
        }

        // Save signatures for RD
        [HttpPost]
        public void SaveSigRD(string rciSig, int id)
        {
            rciSig = rciSig.ToLower().Trim();
            var rci = db.RCI.Where(m => m.rciID == id).FirstOrDefault();
            var gordonID = (string)TempData["id"];
            var username = rciInputService.GetUsername(gordonID).ToLower().Trim();
            if (rciSig == username)
            {
                rci.checkinSigRD = DateTime.Today;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// If an rci form was submitted, the method loops through it and creates Damage records for the damages the user entered.
        /// If the user chose to delete some existing damages, the method loops through the damages the user wanted to delete and removes them from the dataabse.
        /// </summary>
        /// <param name="rci">The data sent to the method.</param>
        /// <returns></returns>
        [HttpPost]
        public void SaveRCI(RCIForm rci)
        {
            // Check if anything was submitted
            if (rci.newDamages != null)
            {
                var toAdd = new List<Damage>();
                // Save of newly added components
                foreach (var damage in rci.newDamages)
                {
                    var newDamage = new Damage { rciComponentID = damage.componentID, damageDescription = damage.damage, damageType = "TEXT" };
                    toAdd.Add(newDamage);
                    //db.Damage.Add(newDamage);  
                }
                db.Damage.AddRange(toAdd);
            }

            // Check if any existing damages were enqueued for deletion
            if (rci.damagesToDelete != null)
            {
                var toDelete = new List<Damage>();
                // Delete all the damages that were enqueued for deletion.
                foreach (var damageID in rci.damagesToDelete)
                {
                    var damage = db.Damage.Find(damageID);
                    toDelete.Add(damage);
                    //db.Damage.Remove(damage);
                }
                db.Damage.RemoveRange(toDelete);
            }

            // Save changes to database
            db.SaveChanges();

            return;
        }

    }
}
