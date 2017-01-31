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

        // This test is currently failing, because an exception is thrown when FindUser
        // gets an invalid ADContext. But I think we can eliminate this test b/c in 
        // LoginController we check that ADContext is not null beforehand
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Given_InvalidADContext_When_ValidUsername_Then_ReturnNull()
        {
            // Arrange
            PrincipalContext ADContext = null;
            var username = "360.StudentTest";

            // Act
            var result = loginService.FindUser(username, ADContext);

            // Assert
        }

        [TestMethod]
        public void IsValidUser_ValidUserEntry_ValidPassword_ReturnTrue()
        {
            // Arrange
            var ADContext = loginService.ConnectToADServer();
            var username = "360.StudentTest";
            var password = "Gordon16";
            var userEntry = loginService.FindUser(username, ADContext);

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
            var userEntry = loginService.FindUser(username, ADContext);

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
    }
}
