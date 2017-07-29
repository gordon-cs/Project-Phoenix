using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Phoenix.Tests.Pages;
using OpenQA.Selenium.Chrome;
using Phoenix.Tests.TestUtilities;
using Phoenix.Services;
using Phoenix.Models;
using System.Linq;

namespace Phoenix.Tests.Tests
{
    [TestClass]
    public class RciGenerationTests
    {
        private IWebDriver wd = new ChromeDriver();
        private RCIContext db = new RCIContext();
        
        /// <summary>
        /// Verify that when an RA logs in, rci records generated in the db = rci records displayed
        ///  Steps:
        ///   - Start by deleting all existing Rcis for that RA's dorm(s)
        ///   - Log the RA in 
        ///   - Verify that the counts match up
        /// </summary>
        [TestMethod]
        public void RciGeneration_Test_1()
        {
            var loginService = new LoginService();
            var dorms = loginService.GetKingdom(Credentials.RA_ID_NUMBER);

            var rcis = db.Rci.Where(m => dorms.Contains(m.BuildingCode));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            // Verify that we have deleted all rcis for this building.
            Assert.AreEqual(db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count(), 0);

            wd.Navigate().GoToUrl(Values.START_URL);
            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.RA_USERNAME);
            login.EnterUserPassword(Credentials.RA_PASSWORD);
            // Submitting the credentials leads us to the dashboard.
            DashboardPage dashboard = login.SubmitCredentials();

            // Verify that the number of rcis displayed matches the number of rcis in the database for the specified dorms.
            Assert.AreEqual(dashboard.RciCount(), db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count());

            wd.Close();
        }

        /// <summary>
        /// Verify that upon second login, no new rcis are added
        /// Steps:
        /// - Log in a first time, then log out.
        /// - Log back in and verify that the number of displayed rcis are the same
        /// </summary>
        [TestMethod]
        public void RciGeneration_Test_2()
        {
            wd.Navigate().GoToUrl(Values.START_URL);

            // Login a first time.
            LoginPage login1 = new LoginPage(wd);
            login1.EnterUsername(Credentials.RA_USERNAME);
            login1.EnterUserPassword(Credentials.RA_PASSWORD);
            DashboardPage dashboard1 = login1.SubmitCredentials();
            var rciCount1 = dashboard1.RciCount();

            // Logout
            var login2 = dashboard1.Logout();

            // Login a second time.
            login2.EnterUsername(Credentials.RA_USERNAME);
            login2.EnterUserPassword(Credentials.RA_PASSWORD);
            DashboardPage dashboard2 = login2.SubmitCredentials();
            var rciCount2 = dashboard2.RciCount();

            // Assert
            Assert.AreEqual(rciCount1, rciCount2);

            wd.Close();
        }


    }
}
