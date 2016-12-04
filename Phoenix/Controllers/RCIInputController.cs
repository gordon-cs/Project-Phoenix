using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Phoenix.Models;

namespace Phoenix.Controllers
{
    public class RCIInputController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private RCIContext db;

        private List<int> damagesToDelete;
        public RCIInputController()
        {
            db = new Models.RCIContext();
            damagesToDelete = new List<int>();
        }

        // GET: RCIInput
        public ActionResult Index()
        {
            var resRCI = db.ResidentRCI.FirstOrDefault();

            return View(resRCI);
        }

        // GET: RCIInput/SaveRCI
        public ActionResult SaveRCI(FormCollection collection)
        {
            var damageList = new Lis
            // Save of newly added components
            foreach (var key in collection.Keys)
            {
                var lol = collection[key];
            }
            // Delete all the damages that were enqueued for deletion.
            damagesToDelete.ForEach(p => db.Damage.Remove(db.Damage.Find(p)));

            return RedirectToAction("Index");
        }

        // GET: RCIInput/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RCIInput/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RCIInput/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RCIInput/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var test = collection;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RCIInput/Delete/5
        public void QueueDamageForDelete(int id)
        {
            if(!ModelState.IsValid)
            {
                // indicate an error
            }
            damagesToDelete.Add(id);
        }



        // POST: RCIInput/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
