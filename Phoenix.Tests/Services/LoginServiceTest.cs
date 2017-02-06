using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.AccountManagement;
using Phoenix.Services;

namespace Phoenix.Tests.Services
{
    [TestClass]
    public class LoginServiceTest
    {
        LoginService loginService = new LoginService();

        [TestMethod]
        public void FindUser_ValidUsername_ReturnUserEntry()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            var username = "360.StudentTest";

            // Act
            var result = loginService.FindUser(username, ADContext);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FindUser_InvalidUsername_ReturnNull()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            var username = "meriadoc.brandybuck";

            // Act
            var result = loginService.FindUser(username, ADContext);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindUser_NullUsername_ReturnNull()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            string username = null;
            // Act
            var result = loginService.FindUser(username, ADContext);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Given_InvalidADContext_When_ValidUsername_Then_ReturnNull()
        {
            // Arrange
            PrincipalContext ADContext = null;
            var username = "360.StudentTest";

            // Act
            var result = loginService.FindUser(username, ADContext);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IsValidUser_ValidUserEntry_ValidPassword_ReturnTrue()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            var username = "360.StudentTest";
            var password = "Gordon16";

            // Act
            bool result = loginService.IsValidUser(username, password, ADContext);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidUser_InValidPassword_ReturnFalse()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            var username = "360.StudentTest";
            var password = "wrongGordon16";

            // Act
            bool result = loginService.IsValidUser(username, password, ADContext);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidUser_NullUsername_ReturnFalse()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            string username = null;
            var password = ""; // The password does not matter in this test case

            // Act
            bool result = loginService.IsValidUser(username, password, ADContext);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidUser_NullPassword_ReturnFalse()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            var username = ""; // The username does not matter in this test case
            string password = null; 

            // Act
            bool result = loginService.IsValidUser(username, password, ADContext);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetRole_ValidRACredentials_ReturnRAasRole()
        {
            // Arrange
            var id = "999999097"; // ID of 360.StudentTest

            // Act
            var result = loginService.GetRole(id);

            // Assert 
            Assert.AreEqual("RA", result);
        }

        [TestMethod]
        public void GetRole_ValidRDCredentials_ReturnRDasRole()
        {
            // Arrange
            var id = "999999098";

            // Act
            var result = loginService.GetRole(id);

            // Assert
            Assert.AreEqual("RD", result);
        }

        [TestMethod]
        public void GetRole_ResidentCredentials_ReturnResidentAsRole()
        {            
            // Arrange
            var id = "50169203";

            // Act
            var result = loginService.GetRole(id);

            // Assert
            Assert.AreEqual("Resident", result);

        }

        [TestMethod]
        public void GetRole_NullID_ReturnsNull()
        {
            // Arrange
            string id = null ;

            // Act
            var result = loginService.GetRole(id);

            // Assert
            Assert.IsNull(result);

        }

        [TestMethod]
        public void GetBuilding_ValidRAID_ReturnCorrectBuilding()
        {
            // Arrange 
            var id = "999999097"; // ID of 360.StudentTest

            // Act 
            var result = loginService.GetBuilding(id);

            // Assert
            Assert.AreEqual("TAV", result);
        }

        [TestMethod]
        public void GetBuilding_ValidRDID_ReturnCorrectBuilding()
        {
            // Arrange 
            var id = "999999098"; // ID of 360.StudentStaff

            // Act 
            var result = loginService.GetBuilding(id);

            // Assert
            Assert.AreEqual("WIL,BRO", result);
        }

        [TestMethod]
        public void GetBuilding_ValidStudentID_ReturnCorrectBuilding()
        {
            // Arrange 
            var id = "50169203";

            // Act 
            var result = loginService.GetBuilding(id);

            // Assert
            Assert.IsTrue(result == "TAV");
        }

        [TestMethod]
        public void GetBuilding_NullID_ReturnNull()
        {
            // Arrange 
            string id = null; 

            // Act 
            var result = loginService.GetBuilding(id);

            // Assert
            Assert.IsNull(result);
        }

    }
}
