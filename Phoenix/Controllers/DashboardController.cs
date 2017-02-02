using System;
using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using Phoenix.Services;

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
            // Look through RCIS and find your RCI with your ID
            // For common area RCI, look through rci's without a gordon id, 
            // with the corresponding Building and Room number

            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];
            var strBuilding = (string)TempData["building"];
            var strRoomNumber = (string)TempData["room"];

            var RCIs = dashboardService.GetRCIsForResident(strID);

            RCIs = (IQueryable<HomeRCIViewModel>)dashboardService.ValidateResidentsRCIsExistence(RCIs, strBuilding, strRoomNumber, strID);

            return View(RCIs);
        }

        // GET: Home/RA
        public ActionResult RA()
        {
            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];
            string strBuilding = (string)TempData["building"];
            var strRoomNumber = (string)TempData["room"];

            // Query just for the RA's own RCI
            var RCIs = dashboardService.GetRCIsForResident(strID);

            // Verify that the RA actually has their own RCIs set up
            dashboardService.ValidateResidentsRCIsExistence(RCIs, strBuilding, strRoomNumber, strID);

            // Display all RCI's for the corresponding building
            string[] strBuildingAsArray = { strBuilding };
            var buildingRCIs = dashboardService.GetRCIsForBuilding(strBuildingAsArray);
            RCIs = RCIs.Concat(buildingRCIs);

            return View(RCIs);
        }

        // GET: Home/RD
        public ActionResult RD()
        {
            // Display all RCI's for the corresponding building

            // TempData stores object, so always cast to string.
            var strBuilding = (string)TempData["building"];

            var buildingRCIs = dashboardService.GetRCIsForBuilding(dashboardService.CollectRDBuildingCodes(strBuilding));
            
            return View(buildingRCIs);
        }

        // Potentially later: admin option that can view all RCI's for all buildings
    }
}