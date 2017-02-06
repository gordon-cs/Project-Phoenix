using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phoenix;
using Phoenix.Controllers;
using Phoenix.Services;

namespace Phoenix.Tests.Controllers
{
    [TestClass]
    public class DashboardControllerTest
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
        public void GetRCIsForResident_ValidResidentIDWithoutRCI_ReturnNull()
        {
            // Arrange 
            var id = "50103344"; // ID of a student who doesn't have an RCI in system

            // Act 
            var result = dashboardService.GetRCIsForResident(id);

            // Assert
            Assert.IsNull(result); // not sure why this fails
        }

        [TestMethod]
        public void GetRCIsForBuilding_ValidBuildingCode_ReturnEntries()
        {
            // Arrange 
            var buildingCode = new string[1] { "WIL" }; 

            // Act 
            var result = dashboardService.GetRCIsForBuilding(buildingCode);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetRCIsForBuilding_InvalidBuildingCode_ReturnNull()
        {
            // Arrange 
            var buildingCode = new string[1] { "HELLO" }; 

            // Act 
            var result = dashboardService.GetRCIsForBuilding(buildingCode);

            // Assert
            Assert.IsNull(result); // not sure why this fails
        }

        [TestMethod]
        public void CollectRDBuildingCodes_ValidJobTitle_ReturnCorrectBuildingCodes()
        {
            // Arrange 
            var jobTitle = "Ferrin and Drew"; 

            // Act 
            var result = dashboardService.CollectRDBuildingCodes(jobTitle);

            // Assert
            Assert.AreEqual(new string[2] { "FER", "DRE" }, result);
        }

        [TestMethod]
        public void CollectRDBuildingCodes_InvalidJobTitle_ReturnNull()
        {
            // Arrange 
            var jobTitle = "Ferrin and Wilson"; 

            // Act 
            var result = dashboardService.CollectRDBuildingCodes(jobTitle);

            // Assert
            Assert.IsNull(result);
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
