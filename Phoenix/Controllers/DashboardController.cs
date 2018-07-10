using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Phoenix.Filters;
using Phoenix.Services;
using Newtonsoft.Json.Linq;
using System;
using Phoenix.Utilities;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class DashboardController : Controller
    {
        private IDashboardService dashboardService;

        public DashboardController(IDashboardService service)
        {
            dashboardService = service;
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
            if (role.Equals(Constants.ADMIN))
            {
                return RedirectToAction("Index", "AdminDashboard");
            }

            else if (role.Equals(Constants.RD))
            {
                return RedirectToAction("RD");
            }
            else if (role.Equals(Constants.RA))
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

            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];
            if (strID == null)
            {
                throw new ArgumentNullException("Couldn't find the Gordon ID for  resident. It was null.");
            }

            var currentBuilding = (string)TempData["currentBuilding"];
            if (currentBuilding == null)
            {
                throw new ArgumentNullException("Couldn't find current building for resident. It was null.");
            }

            var currentRoom = (string)TempData["currentRoom"];
            if (currentRoom == null)
            {
                throw new ArgumentNullException("Couldn't find current room for resident. It was null.");
            }

            var temp = (JValue)TempData["currentRoomAssignDate"];
            if (temp == null)
            {
                throw new ArgumentNullException("Couldn't get the most recent room assign date for resident. It was null");
            }

            DateTime currentRoomAssignDate = temp.ToObject<DateTime>();

            dashboardService.SyncRoomRcisFor(currentBuilding, currentRoom, strID, currentRoomAssignDate);
            dashboardService.SyncCommonAreaRcisFor(currentBuilding, currentRoom);

            var RCIs = dashboardService.GetCurrentRcisForResident(strID);
            var commonAreaRcis = dashboardService.GetCurrentCommonAreaRcisForRoom(currentRoom, currentBuilding);

            return View(RCIs.Concat(commonAreaRcis));
        }

        // GET: Home/RA
        [ResLifeStaff]
        public ActionResult RA()
        {
            // Redirect to login if role not correct
            var role = (string)TempData["role"];
            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();
            dashboardService.SyncRoomRcisFor(kingdom);
            var buildingRCIs = dashboardService.GetCurrentRcisForBuilding(kingdom);

            return View(buildingRCIs);
        }

        // GET: Home/RD
        [RD]
        public ActionResult RD()
        {
            // Redirect to login if role not correct
            var role = (string)TempData["role"];
            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Display all RCI's for the corresponding building
            // RD is not in RoomAssign, so there will be nothing under currentRoomNumber and currentBuilding.
            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();
            dashboardService.SyncRoomRcisFor(kingdom);
            var buildingRcis = dashboardService.GetCurrentRcisForBuilding(kingdom);
            
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

            var buildingRcis = dashboardService.GetCurrentRcisForBuilding(kingdom);

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

        /// <summary>
        /// Export all the fines and charges recorded to a spreadsheet
        /// </summary>
        /// <returns>A .csv file sent back to the client</returns>
        [RD]
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