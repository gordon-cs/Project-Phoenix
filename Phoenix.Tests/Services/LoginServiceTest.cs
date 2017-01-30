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
        public void Given_ADContext_When_ValidUserEntry_Then_ReturnUserEntry()
        {
            // Arrange
            //var ADContext = loginService.ConnectToADServer();
            
            // Act
            // Assert
        }
    }
}
