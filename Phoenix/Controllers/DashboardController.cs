using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Phoenix.Filters;
using Phoenix.Services;
using Newtonsoft.Json.Linq;
using System;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class DashboardController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private DashboardService dashboardService;

        public DashboardController()
        {
            
            dashboardService = new DashboardService();
        }

        // GET: Home
        public ActionResult Index()
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (role.Equals("RD"))
            {
                return RedirectToAction("RD");
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("RA");
            }
            else
            {

                return RedirectToAction("Resident" );
            }
          
        }

        // GET: Home/Resident
        public ActionResult Resident()
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (role.Equals("RD"))
            {
                return RedirectToAction("RD");
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("RA");
            }

            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];
            var currentBuilding = (string)TempData["currentBuilding"];
            var currentRoom = (string)TempData["currentRoom"];
            var temp = (JValue)TempData["currentRoomAssignDate"];
            DateTime currentRoomAssignDate = temp.ToObject<DateTime>();

            dashboardService.SyncRoomRcisFor(currentBuilding, currentRoom, strID, currentRoomAssignDate);
            dashboardService.SyncCommonAreaRcisFor(currentBuilding, currentRoom);

            var RCIs = dashboardService.GetRcisForResident(strID);
            var commonAreaRcis = dashboardService.GetCommonAreaRci(currentRoom, currentBuilding);

            return View(RCIs.Concat(commonAreaRcis));
        }

        // GET: Home/RA
        public ActionResult RA()
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            if (role.Equals("RD"))
            {
                return RedirectToAction("RD");
            }
            else if (role.Equals("Resident"))
            {
                return RedirectToAction("Resident");
            }

            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            var buildingRCIs = dashboardService.GetRcisForBuilding(kingdom);

            return View(buildingRCIs);
        }

        // GET: Home/RD
        public ActionResult RD()
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "LoginController");
            }

            if (role.Equals("Resident"))
            {
                return RedirectToAction("Resident");
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("RA");
            }

            // Display all RCI's for the corresponding building

            // RD is not in RoomAssign, so there will be nothing under currentRoomNumber and currentBuilding.
            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            var buildingRcis = dashboardService.GetRcisForBuilding(kingdom);
            
            return View(buildingRcis);
        }

        public ActionResult GotoRci(int rciID)
        {
            var state = dashboardService.GetRciState(rciID);
            var role = (string)TempData["role"];

            var routeToTake = dashboardService.GetRciRouteDictionary(rciID);

            return routeToTake[state][role];
                
        }

        /// <summary>
        /// Return the ArchiveRcis View to user.
        /// </summary>
        [RD]
        [HttpGet]
        public ActionResult ArchiveRcis()
        {
            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            var buildingRcis = dashboardService.GetRcisForBuilding(kingdom);

            return View(buildingRcis);
        }

        /// <summary>
        /// Receives a list of rcis and sets their IsCurrent column to false.
        /// </summary>
        [RD]
        [HttpPost]
        public void ArchiveRcis(List<int> rciID)
        {
            dashboardService.ArchiveRcis(rciID);
        }

        [HttpGet]
        public ActionResult SyncRcis()
        {
            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            dashboardService.SyncRoomRcisFor(kingdom);
            dashboardService.SyncCommonAreaRcisFor(kingdom);
            return RedirectToAction("Index");
        }

        // Potentially later: admin option that can view all RCI's for all buildings

        // Maybe use an authorization filter here to only allow an RD to access this method?
        // It would be kind of filtered by default since the Export Fines button only appears in RD's view
        // But I suppose if someone new the path, they could call this controller method just from the url
        [HttpGet]
        public FileContentResult ExportFines()
        {
            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            string finesString = dashboardService.GenerateFinesSpreadsheet(kingdom);

            string filename = "fines.csv";

            return File(new System.Text.UTF8Encoding().GetBytes(finesString), "text/csv", filename);
        }
    }
}