using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using System.Diagnostics;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RCIInputController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private RCIContext db;

        // This list is static so it will persist across actions.
        private static List<int> damagesToDelete = new List<int>();

        public RCIInputController()
        {
            Debug.WriteLine("Initialize RCIInput Controller");
            db = new Models.RCIContext();
        }

        public ActionResult Index()
        {
            Debug.WriteLine("Reached Index Method for RCIInput Controller");

            // This is how we access items set in the filter.
            ViewBag.Name = TempData["user"];

            var resRCI = db.ResidentRCI.FirstOrDefault();

            return View(resRCI);
        }

        /// <summary>
        /// If an rci form was submitted, the method loops through it and creates Damage records for the damages the user entered.
        /// If the user chose to delete some existing damages, the method loops through the damages the user wanted to delete and removes them from the dataabse.
        /// </summary>
        /// <param name="rci">The data sent to the method.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRCI(List<RCIForm> rci)
        {
            // Check if anything was submitted
            if (rci != null)
            {
                var toAdd = new List<Damage>();
                // Save of newly added components
                foreach (var damage in rci)
                {
                    var newDamage = new Damage { RCIComponentID = damage.name, DamageDescription = damage.value };
                    toAdd.Add(newDamage);
                    //db.Damage.Add(newDamage);  
                }
                db.Damage.AddRange(toAdd);
            }

            // Check if any existing damages were enqueued for deletion
            if (damagesToDelete.Any())
            {
                var toDelete = new List<Damage>();
                // Delete all the damages that were enqueued for deletion.
                foreach (var damageID in damagesToDelete)
                {
                    var damage = db.Damage.Find(damageID);
                    toDelete.Add(damage);
                    //db.Damage.Remove(damage);
                }
                db.Damage.RemoveRange(toDelete);
            }

            // Clear the queue
            damagesToDelete.Clear();

            // Save changes to database
            db.SaveChanges();

            /* Since SaveRCI is called with javascript, it returns its value (the Index View) to the javascript that called it and not directly to the browser. */
            return RedirectToAction("Index");
        }

        // GET: RCIInput/Delete/5
        [HttpPost]
        public void QueueDamageForDelete(int id)
        {
            if(!ModelState.IsValid)
            {
                // indicate an error
            }
            damagesToDelete.Add(id);
        }

    }
}
