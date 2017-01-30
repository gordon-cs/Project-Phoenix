using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices;
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
            var username = "stephanie.powers";

            // Act
            var result = loginService.FindUser(username, ADContext);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Given_ADContext_When_InvalidUsername_Then_ReturnNull()
        {
            // Arrange
            //var ADContext = loginService.ConnectToADServer();

            // Act

            // Assert
        }
    }
}
