using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using Phoenix.Services;
using System.IO;
using Newtonsoft.Json.Linq;

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
            if(role.Equals("ADMIN"))
            {
                return RedirectToAction("Admin");
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

            // Look through RCIS and find your RCI with your ID
            // For common area RCI, look through rci's without a gordon id, 
            // with the corresponding Building and Room number

            // TempData stores object, so always cast to string.

            var strID = (string)TempData["id"];
            var currentBuilding = (string)TempData["currentBuilding"];
            var currentRoom = (string)TempData["currentRoom"];

            var RCIs = dashboardService.GetRcisForResident(strID);

            if (!dashboardService.CurrentRciExists(RCIs, currentBuilding, currentRoom))
            {
                var rciId = dashboardService.GenerateOneRCIinDb(currentBuilding, currentRoom, strID);
                dashboardService.AddRciComponents(rciId, "dorm room");
            }

            if (currentBuilding.Equals("BRO") || currentBuilding.Equals("TAV") ||
                (currentBuilding.Equals("FER") && (currentRoom.StartsWith("L"))))
            {
                var apartmentNumber = currentRoom.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });

                var commonAreaRCIs = dashboardService.GetCommonAreaRci(apartmentNumber, currentBuilding);

                // If there was no common area RCI for someone in BRO, TAV, or FER apts, then add one
                if (!commonAreaRCIs.Any())
                {
                    

                    var rciId = dashboardService.GenerateOneRCIinDb(currentBuilding, apartmentNumber);
                    dashboardService.AddRciComponents(rciId, "common area");
                }

                RCIs = RCIs.Concat(commonAreaRCIs);
            }


            return View(RCIs);
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

            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];
            var currentBuilding = (string)TempData["currentBuilding"];
            var currentRoom = (string)TempData["currentRoom"];

            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            // Query just for the RA's own RCI
            var RCIs = dashboardService.GetRcisForResident(strID);

            // Verify that the RA actually has their own RCIs set up
            if (!dashboardService.CurrentRciExists(RCIs, currentBuilding, currentRoom))
            {
                var rciId = dashboardService.GenerateOneRCIinDb(currentBuilding, currentRoom, strID);
                dashboardService.AddRciComponents(rciId, "dorm room");
            }

            if (currentBuilding.Equals("BRO") || currentBuilding.Equals("TAV") ||
                (currentBuilding.Equals("FER") && (currentRoom.StartsWith("L"))))
            {
                var apartmentNumber = currentRoom.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });
                var commonAreaRCIs = dashboardService.GetCommonAreaRci(apartmentNumber, currentBuilding);

                // If there was no common area RCI for someone in BRO, TAV, or FER apts, then add one
                if (!commonAreaRCIs.Any())
                {

                    var rciId = dashboardService.GenerateOneRCIinDb(currentBuilding, apartmentNumber);
                    dashboardService.AddRciComponents(rciId, "common area");
                }

            }

            // Also display all RCI's for the corresponding building
            //string[] strBuildingAsArray = { strBuilding };

            var buildingRCIs = dashboardService.GetCurrentRcisForBuilding(kingdom, strID);
            RCIs = RCIs.Concat(buildingRCIs);

            return View(RCIs);
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

            var strID = (string)TempData["id"];

            var buildingRCIs = dashboardService.GetCurrentRcisForBuilding(kingdom, strID);
            
            return View(buildingRCIs);
        }

        public ActionResult Admin()
        {
            // Display all RCI's 
            var temp = (JArray)TempData["kingdom"];
            List<string> kingdom = temp.ToObject<List<string>>();

            var buildingRCIs = dashboardService.GetAllRcisForBuilding(kingdom);

            return View(buildingRCIs);
        }

        // Potentially later: admin option that can view all RCI's for all buildings

        // Maybe use an authorization filter here to only allow an RD to access this method?
        // It would be kind of filtered by default since the Export Fines button only appears in RD's view
        // But I suppose if someone new the path, they could call this controller method just from the url
        [HttpGet]
        public FileContentResult ExportFines()
        {
            //string[] buildingCodes = dashboardService.CollectRDBuildingCodes((string)TempData["building"]);
            var kingdom = (List<string>)TempData["kingdom"];

            string finesString = dashboardService.GenerateFinesSpreadsheet(kingdom);

            string filename = "fines.csv";

            return File(new System.Text.UTF8Encoding().GetBytes(finesString), "text/csv", filename);
        }
    }
}