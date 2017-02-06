using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class RCICheckoutController : Controller
    {
        private RCIContext db;

        private static List<int> finesToDelete = new List<int>();

        public RCICheckoutController()
        {
            db = new Models.RCIContext();
        }

        // GET: RCICheckout
        /// <summary>
        /// Dislay the checkout view for the specified rci
        /// </summary>
        /// <param name="id">RCI identifier</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var rci = db.RCI.Where(m => m.RCIID == id).FirstOrDefault();
            if (rci.GordonID == null) // A common area rci
            {
                ViewBag.ViewTitle = rci.BuildingCode + rci.RoomNumber + " Common Area";
            }
            else
            {
                var name = db.Account.Where(m => m.ID_NUM == rci.GordonID)
                    .Select(m => m.firstname + " " + m.lastname).FirstOrDefault();
                ViewBag.ViewTitle = rci.BuildingCode + rci.RoomNumber + " " + name;
            }

            return View(rci);
        }

        public void SaveRCI(RCIFinesForm rci)
        {
            // Check if anything was submitted
            if (rci.newFines != null)
            {
                var toAdd = new List<Fine>();

                foreach (var fine in rci.newFines)
                {
                    var newFine = new Fine { RCIComponentID = fine.componentId, Reason = fine.fineReason, FineAmount = fine.fineAmount };
                    toAdd.Add(newFine);
                }
                db.Fine.AddRange(toAdd);
            }

            // Check if any existing fines were enqueued for deletion
            if (finesToDelete.Any())
            {
                var toDelete = new List<Fine>();
                // Delete all the fines that were enqueued for deletion.
                foreach (var fineID in finesToDelete)
                {
                    var fine = db.Fine.Find(fineID);
                    toDelete.Add(fine);
                }
                db.Fine.RemoveRange(toDelete);
            }

            // Clear the queue
            finesToDelete.Clear();

            // Save changes to database
            db.SaveChanges();

            return;
        }

        // GET: RCICheckout/Delete/5
        [HttpPost]
        public void QueueFineForDelete(int id)
        {
            if (!ModelState.IsValid)
            {
                // indicate an error
            }
            finesToDelete.Add(id);
        }
    }
}