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
        public void Given_ADContext_When_ValidUsername_Then_ReturnUserEntry()
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
        public void Given_ADContext_When_InvalidUsername_Then_ReturnNull()
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
        public void Given_ADContext_and_ValidUserEntry_When_ValidPassword_Then_ReturnNull()
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
        public void Given_ADContext_and_ValidUserEntry_When_InValidPassword_Then_ReturnNull()
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
    }
}
