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
        public void RciGeneration_Dorm_Res_LogIn_FirstTime()
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

            var rciCard = dashboard.GetRciCardWithName(resident_name["firstname"] + " " + resident_name["lastname"]);

            wd.Close();
        }

        /// <summary>
        /// Verity that when an apartment resident logs in for the first time, 
        ///  their individual and common area rcis are present.
        /// Steps:
        /// - Start by deleting any existing Rcis for the resident (to simulate first login)
        /// - Login
        /// - Verify that the Rci displayed belongs to the resident.
        /// - Verify that the Common area rci is present.
        /// - Verify that the Common area rci is for the correct Room
        /// </summary>
        [TestMethod]
        public void RciGeneration_Apt_Res_LogIn_FirstTime()
        {
            // Delete existing rcis
            var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER));
            var oldSignatures = db.CommonAreaRciSignature.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER));
            db.Rci.RemoveRange(oldRcis);
            db.CommonAreaRciSignature.RemoveRange(oldSignatures);
            db.SaveChanges();

            // Login
            wd.Navigate().GoToUrl(Values.START_URL);
            var dashboard = new LoginPage(wd).LoginAs(
                                                Credentials.APT_RES_1_USERNAME,
                                                Credentials.APT_RES_1_PASSWORD);

            // Assert that the rci count is 2
            Assert.IsTrue(dashboard.RciCount().Equals(2), "Expected two rcis to be present. Found " + dashboard.RciCount());

            // Try to find the rci card with the resident's name.
            var resident_name = Methods.GetFullName(Credentials.APT_RES_1_ID_NUMBER);
            var roomID = Methods.GetRoomID(Credentials.APT_RES_1_ID_NUMBER);

            // The following will throw exceptions if the cards are note found :D
            var rciCard = dashboard.GetRciCardWithName(resident_name["firstname"] + " " + resident_name["lastname"]);
            var commonAreaRciCard = dashboard.GetCommonAreaRciCard(roomID["building"] + " " + roomID["room"].TrimEnd(new char[] { 'A', 'B', 'C', 'D' }));

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
        public void RciGeneration_Dorm_Res_LogIn_SecondTime()
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
            Assert.AreEqual(count1, count2, "The number of rcis changed between first and second login of the dorm resident.");

            wd.Close();
        }

        /// <summary>
        /// Verify that when an apartment resident logs in a second time, no new rcis are generated.
        /// Steps:
        /// - Start by deleting any existing Rcis for the resident (to simulate first login)
        /// - Login
        /// - Take note of number of rcis present on dashboard
        /// - Logout
        /// - Login again and take note of number of rcis present on dashboard
        /// - Asset that the number of rcis present on the dashboard did not change.
        /// </summary>
        [TestMethod]
        public void RciGeneration_Apt_Res_LogIn_SecondTime()
        {
            // Clear old rcis.
            var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER));
            var oldSignatures = db.CommonAreaRciSignature.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER));
            db.Rci.RemoveRange(oldRcis);
            db.CommonAreaRciSignature.RemoveRange(oldSignatures);
            db.SaveChanges();

            // Login
            wd.Navigate().GoToUrl(Values.START_URL);
            var loginPage = new LoginPage(wd);
            var dashboard = loginPage.LoginAs(Credentials.APT_RES_1_USERNAME,
                                                                    Credentials.APT_RES_1_PASSWORD);

            // Assert that the rci count is 2
            var count1 = dashboard.RciCount();

            dashboard.Logout();

            loginPage.LoginAs( Credentials.APT_RES_1_USERNAME,
                                        Credentials.APT_RES_1_PASSWORD);

            var count2 = dashboard.RciCount();

            Assert.AreEqual(count1, count2, "The number of rcis changed between first and second login of the apartment resident.");

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
        public void RciGeneration_Dorm_RA_LogIn_FirstTime()
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
            Assert.AreEqual(dashboard.RciCount(), db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count(), "The number of rcis displayed is different form the number of rcis in the database.");

            wd.Close();
        }

        /// <summary>
        /// Verify that when an AC logs in, rci records generated in the db = rci records displayed
        ///  Steps:
        ///   - Start by deleting all existing Rcis for that RA's dorm(s)
        ///   - Log the RA in 
        ///   - Verify that the counts match up
        ///   - Verify that the AC's room shows up
        ///   - Verify that the AC's common area shows up
        /// </summary>
        [TestMethod]
        public void RciGeneration_Apt_AC_LogIn_FirstTime()
        {
            // Remove old rcis
            var loginService = new LoginService();
            var dorms = loginService.GetKingdom(Credentials.APT_RA_ID_NUMBER);
            var rcis = db.Rci.Where(m => dorms.Contains(m.BuildingCode));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            wd.Navigate().GoToUrl(Values.START_URL);
            LoginPage login = new LoginPage(wd);
            var dashboard = login.LoginAs(Credentials.APT_RA_USERNAME,
                                                            Credentials.APT_RA_PASSWORD);

            // Verify that the number of rcis displayed matches the number of rcis in the database for the specified dorms.
            Assert.AreEqual(dashboard.RciCount(), db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count(), "The number of rcis displayed is different form the number of rcis in the database.");

            var raName = Methods.GetFullName(Credentials.APT_RA_ID_NUMBER);
            var raRoom = Methods.GetRoomID(Credentials.APT_RA_ID_NUMBER);

            // Will throw exception if these cards are not found
            dashboard.GetRciCardWithName(raName["firstname"] + " " + raName["lastname"]);
            dashboard.GetCommonAreaRciCard(raRoom["building"] + " " + raRoom["room"].TrimEnd(new char[] { 'A', 'B', 'C', 'D' }));


            wd.Close();
        }

        /// <summary>
        /// Verify that upon second login, no new rcis are added
        /// Steps:
        /// - Log in a first time, then log out.
        /// - Log back in and verify that the number of displayed rcis are the same
        /// </summary>
        [TestMethod]
        public void RciGeneration_Dorm_RA_LogIn_SecondTime()
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
            Assert.AreEqual(rciCount1, rciCount2, "The number of rcis changed between first and second login of the ra");

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
        public void RciGeneration_Dorm_RD_LogIn_FirstTime()
        {
            var loginService = new LoginService();
            var dorms = loginService.GetKingdom(Credentials.DORM_RD_ID_NUMBER);

            var rcis = db.Rci.Where(m => dorms.Contains(m.BuildingCode));
            db.Rci.RemoveRange(rcis);
            db.SaveChanges();

            wd.Navigate().GoToUrl(Values.START_URL);
            LoginPage login = new LoginPage(wd);
            var dashboard = login.LoginAs(Credentials.DORM_RD_USERNAME, 
                                                            Credentials.DORM_RD_PASSWORD);
           
            Assert.AreEqual(dashboard.RciCount(), db.Rci.Where(m => dorms.Contains(m.BuildingCode)).Count(), "The number of rcis displayd is different from the number of rcis in the database.");

            wd.Close();
        }

        /// <summary>
        /// Verify that upon second login for an RD, no new rcis are added
        /// Steps:
        /// - Log in a first time, then log out.
        /// - Log back in and verify that the number of displayed rcis are the same
        /// </summary>
        [TestMethod]
        public void RciGeneration_Dorm_RD_LogIn_SecondTime()
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
            Assert.AreEqual(rciCount1, rciCount2, "The number of rcis changed between first and second login of the rd");

            wd.Close();
        }


    }
}
