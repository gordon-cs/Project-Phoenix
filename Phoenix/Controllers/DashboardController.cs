﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using Phoenix.Services;
using System.IO;

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

            //RCIs = (IQueryable<HomeRCIViewModel>)dashboardService.ValidateResidentsRCIsExistence(RCIs, strBuilding, strRoomNumber, strID);
            if (!dashboardService.CurrentRCIisCorrect(RCIs, strBuilding, strRoomNumber))
            {
                var rciId = dashboardService.GenerateOneRCIinDb(strBuilding, strRoomNumber, strID);
                dashboardService.AddRCIComponents(rciId, "dorm room");
            }

            if (strBuilding.Equals("BRO") || strBuilding.Equals("TAV") ||
                (strBuilding.Equals("FER") && (strRoomNumber.StartsWith("L"))))
            {

                var commonAreaRCIs = dashboardService.GetCommonAreaRCI(strRoomNumber, strBuilding);

                // If there was no common area RCI for someone in BRO, TAV, or FER apts, then add one
                if (!commonAreaRCIs.Any())
                {

                    var rciId = dashboardService.GenerateOneRCIinDb(strBuilding, strRoomNumber);
                    dashboardService.AddRCIComponents(rciId, "common area");
                }

                RCIs = RCIs.Concat(commonAreaRCIs);
            }


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
            if (!dashboardService.CurrentRCIisCorrect(RCIs, strBuilding, strRoomNumber))
            {
                var rciId = dashboardService.GenerateOneRCIinDb(strBuilding, strRoomNumber, strID);
                dashboardService.AddRCIComponents(rciId, "dorm room");
            }

            if (strBuilding.Equals("BRO") || strBuilding.Equals("TAV") ||
                (strBuilding.Equals("FER") && (strRoomNumber.StartsWith("L"))))
            {

                var commonAreaRCIs = dashboardService.GetCommonAreaRCI(strRoomNumber, strBuilding);

                // If there was no common area RCI for someone in BRO, TAV, or FER apts, then add one
                if (!commonAreaRCIs.Any())
                {

                    var rciId = dashboardService.GenerateOneRCIinDb(strBuilding, strRoomNumber);
                    dashboardService.AddRCIComponents(rciId, "common area");
                }

                RCIs = RCIs.Concat(commonAreaRCIs);
            }

            // Also display all RCI's for the corresponding building
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

        // Maybe use an authorization filter here to only allow an RD to access this method?
        // It would be kind of filtered by default since the Export Fines button only appears in RD's view
        // But I suppose if someone new the path, they could call this controller method just from the url
        [HttpGet]
        public FileContentResult ExportFines()
        {
            string[] buildingCodes = dashboardService.CollectRDBuildingCodes((string)TempData["building"]);

            string finesString = dashboardService.GenerateFinesSpreadsheet(buildingCodes);
            string filename = "fines.csv";
            //FileInfo fileInfo = new FileInfo(filename);

            //if (!fileInfo.Exists)
            //{
            //    using (StreamWriter writer = fileInfo.CreateText())
            //    {
            //        writer.Write(finesString);

            //    }
            //}

            return File(new System.Text.UTF8Encoding().GetBytes(finesString), "text/csv", filename);
        }
    }
}