﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phoenix;
using Phoenix.Controllers;
using Phoenix.Services;
using Phoenix.Models.ViewModels;

namespace Phoenix.Tests.Services
{
    [TestClass]
    public class DashboardServiceTest
    {
        DashboardService dashboardService = new DashboardService();

        [TestMethod]
        public void GetRCIsForResident_ValidResidentIDWithRCI_ReturnRCIEntry()
        {
            // Arrange 
            var id = "999999097"; // ID of 360.StudentTest

            // Act 
            var result = dashboardService.GetRCIsForResident(id);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetRCIsForResident_ValidResidentIDWithoutRCI_ReturnEmptyList()
        {
            // Arrange 
            var id = "50103344"; // ID of a student who doesn't have an RCI in system

            // Act 
            var result = dashboardService.GetRCIsForResident(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<HomeRCIViewModel>));
            Assert.AreEqual(result.Any(), false);
        }

        [TestMethod]
        public void GetRCIsForBuilding_ValidBuildingCode_ReturnEntries()
        {
            // Arrange 
            var buildingCode = new string[1] { "WIL" };
            var gordonID = "50153295";

            // Act 
            var result = dashboardService.GetRCIsForBuilding(buildingCode, gordonID);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetRCIsForBuilding_InvalidBuildingCode_ReturnEmptyList()
        {
            // Arrange 
            var buildingCode = new string[1] { "HELLO" };
            var gordonID = "50153295";

            // Act 
            var result = dashboardService.GetRCIsForBuilding(buildingCode, gordonID);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<HomeRCIViewModel>));
            Assert.AreEqual(result.Any(), false);
        }

        [TestMethod]
        public void CollectRDBuildingCodes_ValidJobTitle_ReturnCorrectBuildingCodes()
        {
            // Arrange 
            var jobTitle = "Ferrin and Drew"; 

            // Act 
            var result = dashboardService.CollectRDBuildingCodes(jobTitle);

            // Assert
            Assert.IsTrue(result.Any() == true);
            Assert.IsTrue(result.Contains("FER") && result.Contains("DRE"));
        }

        [TestMethod]
        public void CollectRDBuildingCodes_InvalidJobTitle_ReturnEmptyArray()
        {
            // Arrange 
            var jobTitle = "Ferrin and Wilson"; 

            // Act 
            var result = dashboardService.CollectRDBuildingCodes(jobTitle);

            // Assert
            Assert.IsInstanceOfType(result, typeof(string[]));
            Assert.IsTrue(result.Any() == false);
        }

        [TestMethod]
        public void GetCommonAreaRCI_ValidApartmentNumberAndBuilding_ReturnEntry()
        {
            // Arrange 
            var apartmentNumber = "109";
            var building = "TAV";

            // Act 
            var result = dashboardService.GetCommonAreaRCI(apartmentNumber, building);

            // Assert
            Assert.IsNotNull(result);
        }

    }
}