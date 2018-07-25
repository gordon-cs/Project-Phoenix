using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Phoenix.Filters;
using System;
using Phoenix.Services;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.IO;
using Phoenix.Utilities;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RciInputController : Controller
    {
        private IRciInputService rciInputService;

        public RciInputController(IRciInputService service)
        {
            rciInputService = service;
        }

        public ActionResult Index(int id)
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            var rci = rciInputService.GetRci(id);

            //  Redirect to the review page if this is already signed by the RD
            if(rci.RdCheckinDate != null)
            {
                return RedirectToAction("RciReview", new { id = id });
            }

            // This is how we access items set in the filter.
            var gordon_id = (string)TempData["id"];

            // Redirect if the rci doesn't belong to the user.
            if (!rci.isViewableBy(gordon_id, role, (string)TempData["currentRoom"], (string)TempData["currentBuilding"]))
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            ViewBag.ViewTitle = $"Check-In: {rci.BuildingCode} {rci.RoomNumber} {rci.FirstName} {rci.LastName}";

            if (rci.GordonId == null) // A common area rci
            {
                ViewBag.RAIsMemberOfApartment = (role == Constants.RA) && (rci.CommonAreaMembers.Where(m => m.GordonId == gordon_id).Any());
            }

            return View(rci);
        }

        /// <summary>
        /// Returns the checkin review view
        /// </summary>
        public ActionResult RciReview(int id)
        {
            var gordon_id = (string)TempData["id"];

            var rci = rciInputService.GetRci(id);

            // Redirect if the rci doesn't belong to the user.
            if (!rci.isViewableBy(gordon_id, (string)TempData["role"], (string)TempData["currentRoom"], (string)TempData["currentBuilding"]))
            {
                return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
            }

            ViewBag.ViewTitle = $"Check-In Review: {rci.BuildingCode} {rci.RoomNumber} {rci.FirstName} {rci.LastName}";

            return View(rci);
        }
       
        [RD]
        public ActionResult SignAllRD()
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];
            var gordonID = (string)TempData["id"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (role.Equals("Resident") || role.Equals("RA"))
            {
                return RedirectToAction("Index", "Dashboard");
            }


            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            var buildingRcis = rciInputService.GetRcisForBuildingThatCanBeSignedByRD(gordonID, kingdom);


            ViewBag.User = (string)TempData["user"];
            return View(buildingRcis);
        }

        [RD]
        [HttpPost]
        public ActionResult SubmitSignAllRD(string rciSig)
        {
            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();
            if (rciSig == user)
            {
                rciInputService.SignRcis(gordonID);
                return Json(Url.Action("Index", "Dashboard"));
            }
            
            return Json(Url.Action("SignAllRD"));
        }

        /// <summary>
        /// Method that accepts a resident's signature on the common area rci. 
        /// If the signature is valid, the resident has signed the common area rci checkin and a record is inserted into 
        /// the CommonAreaSignatures table.
        /// If everyone has signed, CheckinSigRes column is updated.
        /// </summary>
        [HttpPost]
        public ActionResult SaveSigCommonArea(int rciID, string rciSig)
        {
            if (rciSig == null || rciSig.Trim() == "")
            {
                return new HttpStatusCodeResult(400, "You didn't enter a signature. If you have already signed, you are all set! If you have not,  please sign as it appears in the text box.");
            }
            else
            {
                rciSig = rciSig.ToLower().Trim();
            }

            var gordonID = (string)TempData["id"];

            var user = ((string)TempData["user"]).ToLower().Trim();

            bool complete = rciInputService.SaveCommonAreaMemberSig(rciSig, user, gordonID, rciID);

            if (complete)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return new HttpStatusCodeResult(400, "There was an error processing your signature. This might be because you made a typo with the signature, please try again.");
            }
        }

        // Save signatures for resident
        [HttpPost]
        public ActionResult SaveSigRes(string rciSig, string lacSig, int id)
        {
            // Both can't be null when submitting.
            if (string.IsNullOrWhiteSpace(rciSig) && string.IsNullOrWhiteSpace(lacSig))
            {
                return new HttpStatusCodeResult(400, "You didn't enter a signature. If you have already signed, you are all set! If you have not,  please sign as it appears in the text box.");
            }

            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            if (lacSig != null) lacSig = lacSig.ToLower().Trim();
            
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();

            bool complete = rciInputService.SaveResSigs(rciSig, lacSig, user, id);

            if (complete)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return new HttpStatusCodeResult(400, "There was an error processing your signature. This might be because there was an error with your signature, please try again.");
            }
        }

        // Save signatures for RA
        [ResLifeStaff]
        [HttpPost]
        public ActionResult SaveSigRA(string rciSig, string rciSigRes, string lacSig, int id)
        {
            // No error checking here yet because it is a bit more complex.
            // Some parameters might be null depending on if the RA is signing his/her rci or not.

            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            if (rciSigRes != null) rciSigRes = rciSigRes.ToLower().Trim();
            if (lacSig != null) lacSig = lacSig.ToLower().Trim();
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();

            var complete = rciInputService.SaveRASigs(rciSig, lacSig, rciSigRes, user, id, gordonID);
            
            if (complete)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return new HttpStatusCodeResult(400, "There was an error processing your signature. This might be because there was an error with your signature, please try again.");
            }
        }
        
        /// <summary>
        /// Save signatures for RD
        /// </summary>
        /// <param name="rciSig">Signature</param>
        /// <param name="id">RCI ID</param>
        /// <returns>Redirect to dashboard if signed or checked</returns>
        [RD]
        [HttpPost]
        public ActionResult SaveSigRD(string rciSig, int id)
        {
            if (string.IsNullOrWhiteSpace(rciSig))
            {
                return new HttpStatusCodeResult(400, "You didn't enter a signature. If you have already signed, you are all set! If you have not,  please sign as it appears in the text box.");
            }
            if (rciSig != null) rciSig = rciSig.ToLower().Trim();
            
            var gordonID = (string)TempData["id"];
            var user = ((string)TempData["user"]).ToLower().Trim();

            var complete = rciInputService.SaveRDSigs(rciSig, user, id, gordonID);

            if (complete)
            {
                return Json(Url.Action("Index", "Dashboard"));
            }
            else
            {
                return new HttpStatusCodeResult(400, "There was an error processing your signature. This might be because there was an error with your signature, please try again.");
            }
        }


        /// <summary>
        /// When an RD wants to check on an RCI upon checking in, after they click the checkbox,
        /// an HttpPost request will be sent here and change CheckinSigRDGordonID but not fill
        /// in the date for CheckinSigRD.
        /// </summary>
        /// <param name="queueRciFlag">1: queue, 0: de-queue</param>
        /// <param name="rciId">RCI ID</param>
        /// <returns></returns>
        [RD]
        [HttpPost]
        public void CheckSigRD(int queueRciFlag, int rciId)
        {
            var gordonID = (string)TempData["id"];
            rciInputService.CheckRcis(queueRciFlag, gordonID, rciId);
        }

        /// <summary>
        /// If a photo(s) of a damage was uploaded, this method first creates a new Damage entry in db, then saves the image to the server
        /// For reference, see: http://codepedia.info/upload-image-using-jquery-ajax-asp-net-c-sharp/#jQuery_ajax_call
        /// </summary>
        [HttpPost]
        public int SavePhoto(int rciId, int roomComponentTypeId)
        {
            try
            {
                foreach (string s in Request.Files)
                {
                    HttpPostedFileBase photoFile = Request.Files[s];
                    string rciComponentId = photoFile.FileName;
                    
                    // First, resize the image, using pattern here: http://www.advancesharp.com/blog/1130/image-gallery-in-asp-net-mvc-with-multiple-file-and-size

                    // Create an Image obj from the file
                    Image origImg = Image.FromStream(photoFile.InputStream);
      
                    Size imgSize = rciInputService.NewImageSize(origImg.Size, new Size(300,300));

                    // Bitmap is a subclass of Image; its constructor can take an Image and new Size, and then creates a new Image scaled to the new size
                    Image resizedImg = new Bitmap(origImg, imgSize);

                    rciInputService.ResizeImage(origImg, resizedImg, imgSize);
                    rciInputService.ApplyExifData(resizedImg);

                    string imageName = "RciComponentId" + rciComponentId + "_DamageId" + Guid.NewGuid(); // newDamage.DamageID.ToString(); // Image names of the format: RciComponent324_DamageId23

                    // Get today's date and format correctly for a folder name
                    string todayProperFormat = DateTime.Today.ToShortDateString().Replace("/", "_");

                    string folderPath = "\\Content\\Images\\Damages\\" + todayProperFormat + "\\";

                    // If no folder with today's date has been created yet, create one. This will do nothing if Directory already exists
                    Directory.CreateDirectory(Server.MapPath(folderPath));

                    string fullPath = folderPath + imageName + ".jpg"; // Not sure exactly where we should store them. This path can change

                    var damageid = rciInputService.SavePhotoDamage(fullPath, rciId, null, roomComponentTypeId);
                    resizedImg.Save(Server.MapPath(fullPath), resizedImg.RawFormat);

                    return damageid;
                }
            }
            catch
            {
                Response.Status = "500 Error saving photo";
            }
            return -1;
        }

        /// <summary>
        /// For a particular damage image, delete the damage entry from the database and delete the photo file from
        /// server, based on the damageId
        /// </summary>
        /// <param name="damageId">The id for the damage that has the associated photo we want to delete</param>
        [HttpPost]
        public ActionResult DeletePhoto(int damageId)
        {
            var damageImpagePath = rciInputService.FetchDamageFilePath(damageId);

            if (string.IsNullOrWhiteSpace(damageImpagePath))
            {
                // No file path. Nothing to do here
            }
            else
            {
                try
                {
                    var filePath = Server.MapPath(damageImpagePath);
                    System.IO.File.Delete(filePath); // Remove from server
                }
                catch (Exception e)
                {
                    return new HttpStatusCodeResult(500, "Error deleting photo. Error message: " + e.Message);
                }
            }

            try
            {
                rciInputService.DeleteDamage(damageId);
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500, "Error deleting photo. Error message: " + e.Message);
            }

            return new HttpStatusCodeResult(200, "Successfully deleted!");
        }

        // save one damage to db and return damage id in response
        [HttpPost]
        public int SaveDamage(int rciId, int roomComponentTypeId, string damageDescription)
        {
            return rciInputService.SaveTextDamage(damageDescription, rciId, null, roomComponentTypeId);
        }

        /// <summary>
        /// For a particular damage text, delete the damage entry from the database based on the damageId
        /// </summary>
        /// <param name="damageId">The id for the damage</param>
        [HttpPost]
        public ActionResult DeleteDamage(int damageID)
        {
            try
            {
                rciInputService.DeleteDamage(damageID);
                return new HttpStatusCodeResult(200, "Successfully deleted!");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, "Error deleting damage description. Error message: " + e.Message);
            }
        }
    }
}
