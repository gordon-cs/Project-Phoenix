using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Phoenix.Filters;
using Phoenix.Services;
using Newtonsoft.Json.Linq;
using System;
using Phoenix.Utilities;
using Phoenix.Models.ViewModels;

namespace Phoenix.Controllers
{
    [ExceptionLog]
    [ControllerLog]
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
            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];

            if (strID == null)
            {
                throw new ArgumentNullException("Couldn't find the Gordon ID for  resident. It was null.");
            }

            var currentBuilding = (string)TempData["currentBuilding"];

            var currentRoom = (string)TempData["currentRoom"];

            // These values will be null for RD, Staff and Faculty.
            // We'll let them log in, we just won't show them anything.
            if (currentBuilding != null && currentRoom != null)
            {
                dashboardService.SyncRoomRcisFor(currentBuilding, currentRoom, strID);

                dashboardService.SyncCommonAreaRcisFor(currentBuilding, currentRoom); 

                var RCIs = dashboardService.GetCurrentRcisForResident(strID);

                var commonAreaRcis = dashboardService.GetCurrentCommonAreaRcisForRoom(currentRoom, currentBuilding);

                return View(RCIs.Concat(commonAreaRcis));
            }
            else
            {
                return View(Enumerable.Empty<HomeRciViewModel>());
            }
        }

        // GET: Home/RA
        [ResLifeStaff]
        public ActionResult RA()
        {
            List<string> kingdom = ((JArray)TempData["kingdom"]).ToObject<List<string>>();

            dashboardService.SyncRoomRcisFor(kingdom);

            var buildingRCIs = dashboardService.GetCurrentRcisForBuilding(kingdom);

            return View(buildingRCIs);
        }

        // GET: Home/RD
        [RD]
        public ActionResult RD()
        {
            // Display all RCI's for the corresponding building
            // RD is not in RoomAssign, so there will be nothing under currentRoomNumber and currentBuilding.
            List<string> kingdom = ((JArray)TempData["kingdom"]).ToObject<List<string>>();

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
            List<string> kingdom = ((JArray)TempData["kingdom"]).ToObject<List<string>>();

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

        [ResLifeStaff]
        [HttpPost]
        public ActionResult SwapRcis(int firstRciId, int secondRciId)
        {
            this.dashboardService.SwapRciDamages(firstRciId, secondRciId);

            return RedirectToAction(controllerName: "Dashboard", actionName: "Index");
        }

        [ResLifeStaff]
        [HttpGet]
        public ActionResult SwapRcis()
        {
            var kingdom = ((JArray)TempData["kingdom"]).ToObject<List<string>>();

            var buildingRcis = dashboardService.GetCurrentRcisForBuilding(kingdom)
                .Where(x => !string.IsNullOrWhiteSpace(x.GordonId)) // We are only interested in individual rooms.
                .Where(x => x.CheckinSigRD == null); // Once the RD signs. No one can swap rcis anymore. It just seems right, but i could be convinced otherwise

            return View(buildingRcis);
        }

        

        /// <summary>
        /// Export all the fines and charges recorded to a spreadsheet
        /// </summary>
        /// <returns>A .csv file sent back to the client</returns>
        [RD]
        [HttpGet]
        public FileContentResult ExportFines()
        {
            List<string> kingdom = ((JArray)TempData["kingdom"]).ToObject<List<string>>();

            string finesString = dashboardService.GenerateFinesSpreadsheet(kingdom);

            string filename = "fines.csv";

            return File(new System.Text.UTF8Encoding().GetBytes(finesString), "text/csv", filename);
        }
    }
}