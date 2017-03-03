using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System;
using Phoenix.Services;
using System.Web.UI;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RciInputController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private RCIContext db;
        private RciInputService rciInputService;

        // This list is static so it will persist across actions.
        private static List<int> damagesToDelete = new List<int>();

        public RciInputController()
        {
            Debug.WriteLine("Initialize RCIInput Controller");
            db = new Models.RCIContext();
            rciInputService = new RciInputService();
        }

        public ActionResult Index(int id)
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            Debug.WriteLine("Reached Index Method for RCIInput Controller");

            // This is how we access items set in the filter.
            var gordon_id = (string)TempData["id"];

            //var rci = db.RCI.Where(m => m.RCIID == id).FirstOrDefault();
            var rci = rciInputService.GetRci(id);
            if (rci.GordonID == null) // A common area rci
            {
                ViewBag.ViewTitle = rci.BuildingCode + rci.RoomNumber + " Common Area";
                // Select rooms of common area RCIs to group the RCIs
                ViewBag.commonRooms = rciInputService.GetCommonRooms(id);
            }
            else
            {
                var name = db.Account.Where(m => m.ID_NUM == rci.GordonID)
                    .Select(m => m.firstname + " " + m.lastname).FirstOrDefault();
                ViewBag.ViewTitle = rci.BuildingCode + rci.RoomNumber + " " + name;
            }

            return View(rci);
        }

        // Redirect to checkin signature page for certain roles.
        public ActionResult CheckinSig(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

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
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            if (role.Equals("RD"))
            {
                return RedirectToAction("CheckinSigRD", new { id = id });
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("CheckinSigRA", new { id = id });
            }

            var rci = rciInputService.GetRci(id);
            ViewBag.User = TempData["user"];
            return View(rci);
        }

        // GET: RCIInput/CheckinSigRA/1
        public ActionResult CheckinSigRA(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            if (role.Equals("RD"))
            {
                return RedirectToAction("CheckinSigRD", new { id = id });
            }
            else if (role.Equals("Resident"))
            {
                return RedirectToAction("CheckinSigRes", new { id = id });
            }

            var rci = rciInputService.GetRci(id);
            var gordonID = (string)TempData["id"];
            ViewBag.User = TempData["user"];
            ViewBag.GordonID = gordonID;
            return View(rci);
        }

        // GET: RCIInput/CheckinSigRD/1
        public ActionResult CheckinSigRD(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            if (role.Equals("Resident"))
            {
                return RedirectToAction("CheckinSigRes", new { id = id });
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("CheckinSigRA", new { id = id });
            }

            var rci = rciInputService.GetRci(id);
            var gordonID = (string)TempData["id"];
            ViewBag.User = TempData["user"];
            return View(rci);
        }

        // Save signatures for resident
        [HttpPost]
        public ActionResult SaveSigRes(string rciSig, string lacSig, int id)
        {
            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            if (lacSig != null) lacSig = lacSig.ToLower().Trim();
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();
            if (rciSig == user)
            {
                rci.CheckinSigRes = DateTime.Today;
            }
            if (lacSig == user)
            {
                rci.LifeAndConductSigRes = DateTime.Today;
            }
            db.SaveChanges();

            if (rci.CheckinSigRes != null && rci.LifeAndConductSigRes != null)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return Json(Url.Action("CheckinSigRes", new { id = id }));
            }
        }

        // Save signatures for RA
        [HttpPost]
        public ActionResult SaveSigRA(string rciSig, string rciSigRes, string lacSig, int id)
        {
            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            if (rciSigRes != null) rciSigRes = rciSigRes.ToLower().Trim();
            if (lacSig != null) lacSig = lacSig.ToLower().Trim();
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();
            if (rciSig == user)
            {
                rci.CheckinSigRA = DateTime.Today;
            }
            if (rciSigRes == user)
            {
                rci.CheckinSigRes = DateTime.Today;
            }
            if (lacSig == user)
            {
                rci.LifeAndConductSigRes = DateTime.Today;
            }
            db.SaveChanges();

            if (rci.CheckinSigRes != null && rci.LifeAndConductSigRes != null && rci.CheckinSigRA != null)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return Json(Url.Action("CheckinSigRA", new { id = id }));
            }
        }

        // Save signatures for RD
        [HttpPost]
        public ActionResult SaveSigRD(string rciSig, int id)
        {
            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();
            if (rciSig == user)
            {
                rci.CheckinSigRD = DateTime.Today;
            }
            db.SaveChanges();

            if (rci.CheckinSigRes != null && rci.CheckinSigRA != null)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return Json(Url.Action("CheckinSigRD", new { id = id }));
            }
        }

        /// <summary>
        /// If an rci form was submitted, the method loops through it and creates Damage records for the damages the user entered.
        /// If the user chose to delete some existing damages, the method loops through the damages the user wanted to delete and removes them from the dataabse.
        /// </summary>
        /// <param name="rci">The data sent to the method.</param>
        /// <returns></returns>
        [HttpPost]
        public void SaveRci(RciForm rci)
        {
            // Check if anything was submitted
            if (rci.NewDamages != null)
            {
                var toAdd = new List<Damage>();
                // Save of newly added components
                foreach (var damage in rci.NewDamages)
                {
                    var newDamage = new Damage { RciComponentID = damage.ComponentID, DamageDescription = damage.Damage, DamageType = "TEXT" };
                    toAdd.Add(newDamage);
                    //db.Damage.Add(newDamage);  
                }
                db.Damage.AddRange(toAdd);
            }

            // Check if any existing damages were enqueued for deletion
            if (rci.DamagesToDelete != null)
            {
                var toDelete = new List<Damage>();
                // Delete all the damages that were enqueued for deletion.
                foreach (var damageID in rci.DamagesToDelete)
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


        /// <summary>
        /// If a photo(s) of a damage was uploaded, this method first creates a new Damage entry in db, then saves the image to the server
        /// For reference, see: http://codepedia.info/upload-image-using-jquery-ajax-asp-net-c-sharp/#jQuery_ajax_call
        /// </summary>
        [HttpPost]
        public void SavePhoto()
        {
            try
            {
                foreach (string s in Request.Files)
                {
                    HttpPostedFileBase photoFile = Request.Files[s];
                    string rciComponent = photoFile.FileName;
                    Debug.Write("Filename identified on client: " + rciComponent);
                    //string fileExtension = photoFile.ContentType;
                    string fileExtension = ".jpg";

                    Damage newDamage = new Damage();
                    newDamage.DamageType = "IMAGE";
                    newDamage.RciComponentID = Convert.ToInt32(rciComponent);

                    db.Damage.Add(newDamage);
                    db.SaveChanges();

                    var damageId = newDamage.DamageID;
                    string imageName = "RciComponentId" + rciComponent + "_DamageId" + newDamage.DamageID.ToString(); // Image names of the format: RciComponent324_DamageId23
                    string imagePath = "\\Content\\Images\\Damages\\" + imageName + fileExtension; // Not sure exactly where we should store them. This path can change

                    // First, resize the image, using pattern here: http://www.advancesharp.com/blog/1130/image-gallery-in-asp-net-mvc-with-multiple-file-and-size

                    // Create an Image obj from the file
                    Image origImg = Image.FromStream(photoFile.InputStream);

                    Size imgSize = rciInputService.NewImageSize(origImg.Size, new Size(300, 300));

                    // Bitmap is a subclass of Image; its constructor can take an Image and new Size, and then creates a new Image scaled to the new size
                    Image resizedImg = new Bitmap(origImg, imgSize);

                    using (Graphics gr = Graphics.FromImage(resizedImg))
                    {
                        gr.SmoothingMode = SmoothingMode.HighQuality;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        gr.DrawImage(origImg, new Rectangle(0, 0, imgSize.Width, imgSize.Height));
                    }

                    resizedImg.Save(Server.MapPath(imagePath), resizedImg.RawFormat);

                    newDamage.DamageImagePath = imagePath;
                    db.SaveChanges();

                    Response.Write(imagePath);
                }
            }
            catch (Exception e)
            {
                Response.Status = "Error saving photo";
                Debug.Write("Error saving photo to database: " + e.Message);
            }
            return;
        }



        [HttpDelete]
        public void DeletePhoto(string imagePath, int componentId) //Hmm, this would be cleaner if we used the damage id
        {
            Damage damage = db.Damage.Where(m => m.DamageImagePath.Equals(imagePath) && m.RciComponentID == (componentId)).FirstOrDefault();
            db.Damage.Remove(damage);

        }
    }
}
