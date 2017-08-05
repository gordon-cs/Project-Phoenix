﻿using System;
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
        // Web Driver
        private IWebDriver wd = new ChromeDriver();

        private RCIContext db = new RCIContext();

        /// <summary>
        /// Verity that when a resident logs in for the first time, their rci is generated.
        /// Steps:
        /// - Start by deleting any existing Rcis for the resident (to simulate first login)
        /// - Login
        /// - Verify that the Rci displayed belongs to the resident.
        /// - Verify that only one rci is displayed.
        /// </summary>
        [TestMethod]
        public void RciGeneration_Res_LogIn_FirstTime()
        {
            // Delete existing rcis
            var rcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            // Login
            wd.Navigate().GoToUrl(Values.START_URL);
            var dashboard = new LoginPage(wd).LoginAs(
                                                Credentials.DORM_RES_USERNAME, 
                                                Credentials.DORM_RES_PASSWORD);

            // Assert that the rci count is 1
            Assert.IsTrue(dashboard.RciCount().Equals(1), "More than one rci was present on first login as a dorm resident.");

            // Assert that the correct rci is displayed.
            var resident_name = Methods.GetFullName(Credentials.DORM_RES_ID_NUMBER);

            var rciCard = dashboard.GetRciCardWithName(resident_name);

            wd.Close();
        }

        /// <summary>
        /// Verify that when a resident logs in a second time, no new rcis are generated.
        /// Steps:
        /// - Start by deleting any existing Rcis for the resident (to simulate first login)
        /// - Login
        /// - Take note of number of rcis present on dashboard
        /// - Logout
        /// - Login again and take note of number of rcis present on dashboard
        /// - Asset that the number of rcis present on the dashboard did not change.
        /// </summary>
        [TestMethod]
        public void RciGeneration_Res_LogIn_SecondTime()
        {
            // Delete existing rcis
            var rcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            // Login
            wd.Navigate().GoToUrl(Values.START_URL);
            var loginPage = new LoginPage(wd);
            var dashboard = loginPage.LoginAs(Credentials.DORM_RES_USERNAME,
                                                                    Credentials.DORM_RES_PASSWORD);

            var count1 = dashboard.RciCount();

            // Logout
            dashboard.Logout();

            // Login a second time
            loginPage.LoginAs(Credentials.DORM_RES_USERNAME,
                                         Credentials.DORM_RES_PASSWORD);

            var count2 = dashboard.RciCount();

            // Assert that the rci count is 1
            Assert.AreEqual(count1, count2, "The number of rcis changed between first and second login.");

            wd.Close();
        }

        /// <summary>
        /// Verify that when an RA logs in, rci records generated in the db = rci records displayed
        ///  Steps:
        ///   - Start by deleting all existing Rcis for that RA's dorm(s)
        ///   - Log the RA in 
        ///   - Verify that the counts match up
        /// </summary>
        [TestMethod]
        public void RciGeneration_RA_LogIn_FirstTime()
        {
            // Remove old rcis
            var loginService = new LoginService();
            var dorms = loginService.GetKingdom(Credentials.DORM_RA_ID_NUMBER);

            var rcis = db.Rci.Where(m => dorms.Contains(m.BuildingCode));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            wd.Navigate().GoToUrl(Values.START_URL);
            LoginPage login = new LoginPage(wd);
            var dashboard = login.LoginAs(Credentials.DORM_RA_USERNAME,
                                                            Credentials.DORM_RA_PASSWORD);

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
        public void RciGeneration_RA_LogIn_SecondTime()
        {
            wd.Navigate().GoToUrl(Values.START_URL);

            // Login a first time.
            LoginPage login = new LoginPage(wd);
            var dashboard = login.LoginAs(Credentials.DORM_RA_USERNAME,
                                                            Credentials.DORM_RA_PASSWORD);
            var rciCount1 = dashboard.RciCount();

            // Logout
            dashboard.Logout();

            // Login a second time.
            login.LoginAs(Credentials.DORM_RA_USERNAME,
                                         Credentials.DORM_RA_PASSWORD);
            var rciCount2 = dashboard.RciCount();

            // Assert
            Assert.AreEqual(rciCount1, rciCount2);

            wd.Close();
        }


        /// <summary>
        /// Verify that when an RD logs in, rci records generated in the db = rci records displayed
        ///  Steps:
        ///   - Start by deleting all existing Rcis for that RD's dorm(s)
        ///   - Log the RD in 
        ///   - Verify that the counts match up
        /// </summary>
        [TestMethod]
        public void RciGeneration_RD_LogIn_FirstTime()
        {
            var loginService = new LoginService();
            var dorms = loginService.GetKingdom(Credentials.DORM_RD_ID_NUMBER);

            var rcis = db.Rci.Where(m => dorms.Contains(m.BuildingCode));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            // Verify that we have deleted all rcis for this building.
            Assert.AreEqual(db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count(), 0);

            wd.Navigate().GoToUrl(Values.START_URL);
            LoginPage login = new LoginPage(wd);
            var dashboard = login.LoginAs(Credentials.DORM_RD_USERNAME, 
                                                            Credentials.DORM_RD_PASSWORD);
           
            Assert.AreEqual(dashboard.RciCount(), db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count());

            wd.Close();
        }

        /// <summary>
        /// Verify that upon second login for an RD, no new rcis are added
        /// Steps:
        /// - Log in a first time, then log out.
        /// - Log back in and verify that the number of displayed rcis are the same
        /// </summary>
        [TestMethod]
        public void RciGeneration_RD_LogIn_SecondTime()
        {
            wd.Navigate().GoToUrl(Values.START_URL);

            // Login a first time.
            LoginPage login = new LoginPage(wd);
            var dashboard = login.LoginAs(Credentials.DORM_RD_USERNAME,
                                                            Credentials.DORM_RD_PASSWORD);
            var rciCount1 = dashboard.RciCount();

            // Logout
            dashboard.Logout();

            // Login a second time.
            login.LoginAs(Credentials.DORM_RD_USERNAME,
                                    Credentials.DORM_RD_PASSWORD);
            var rciCount2 = dashboard.RciCount();

            // Assert
            Assert.AreEqual(rciCount1, rciCount2);

            wd.Close();
        }


    }
}
