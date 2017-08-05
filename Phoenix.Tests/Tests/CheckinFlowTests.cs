using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phoenix.Models;
using Phoenix.Tests.TestUtilities;
using Phoenix.Tests.Pages;
using System.Linq;
using System.Collections.Generic;

namespace Phoenix.Tests.Tests
{
    [TestClass]
    public class CheckinFlowTests
    {
        private IWebDriver wd = new ChromeDriver();
        private RCIContext db = new RCIContext();

        /// <summary>
        /// Go through the checkin flow for rcis.
        /// Steps:
        /// - Delete any previous rcis resident may have.
        /// - Login as resident, then logout. (This generates a new rci for the resident)
        /// - Log in as the RA
        /// - Verify the resident's rci  is blue and has no signatures, then logout.
        /// - Login again as the resident.
        /// - Sign the rci, then logout.
        /// - Login as the RA
        /// - Verify that rci is blue and has RES signature
        /// - Sign the rci as the RA
        /// - Verify that rci is blue and has RES and RA signatures
        /// - Login as RD
        /// - Sign the rci as an RD
        /// - Verify that rci is green and has no blocks.
        /// </summary>
        [TestMethod]
        public void CheckinFlow_DormBuilding()
        {
            // Clear old rcis
            var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
            db.Rci.RemoveRange(oldRcis);
            db.SaveChanges();

            wd.Navigate().GoToUrl(Values.START_URL);
            // Resident logs in
            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.DORM_RES_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RES_PASSWORD);
            DashboardPage dashboard = login.SubmitCredentials();

            dashboard.Logout();

            // RA logs in
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            // Submitting the credentials leads us to the dashboard.
            login.SubmitCredentials();

            var res_acct = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RES_ID_NUMBER)).First();
            var resident_name = res_acct.firstname + " " + res_acct.lastname;
            var rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isUnsigned());
            Assert.IsFalse(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());

            // Login again as resident.
            dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RES_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RES_PASSWORD);
            login.SubmitCredentials();

            GenericRciPage rci = dashboard.SelectFirstRciWithName(resident_name);
            rci.asRciCheckinPage().HitNextToSignatures().Sign(resident_name).SubmitSignature();

            dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            login.SubmitCredentials();

            rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());

            // RA signs
            var checkinRci = dashboard.SelectFirstRciWithName(resident_name).asRciCheckinPage();
            var user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RA_ID_NUMBER)).First();
            var userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsTrue(rciCard.isSignedByRA());

            //RD logs in
            dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RD_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RD_PASSWORD);
            login.SubmitCredentials();

            checkinRci = dashboard.SelectFirstRciWithName(resident_name).asRciCheckinPage();

            // RD signs
            user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RD_ID_NUMBER)).First();
            userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsFalse(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());
            Assert.IsFalse(rciCard.isSignedByRD());


            // Cleanup 
            var usedRci = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
            db.Rci.RemoveRange(usedRci);
            db.SaveChanges();
            wd.Quit();
        }

        /// <summary>
        /// Go through the checkin flow for appartment rcis
        /// Steps:
        /// - Delete any previous rcis associated with the residents as well as the common area rci.
        /// - Login as each resident, then logout.
        /// - Log in as the RA
        /// - Verify that all rcis for those residents are blue. Logout
        /// - Login again as each resident.
        /// - Sign the personal and common area rci. Logout.
        /// - Login as the RA
        /// - Verify that all the rcis are blue and have RES signature
        /// - Sign all the rcis as the RA (including the common area one).
        /// - Verify that all rcis are blue and have RES and RA signatures
        /// - Login as RD
        /// - Sign all the rcis as an RD
        /// - Verify that all rcis are green and have no blocks.
        /// </summary>
        [TestMethod]
        public void CheckinFlow_ApartmentBuilding()
        {
            // Clear old rcis
            var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER)
                                                            || m.GordonID.Equals(Credentials.APT_RES_2_ID_NUMBER)
                                                            || m.GordonID.Equals(Credentials.APT_RES_3_ID_NUMBER)
                                                            || m.GordonID.Equals(Credentials.APT_RES_4_ID_NUMBER));
            // Clear the common area rci.
            if(oldRcis.Any())
            {
                var buildingCode = oldRcis.First().BuildingCode;
                var roomNumber = oldRcis.First().RoomNumber;
                var apartmentNumber = roomNumber.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });

                oldRcis.Concat(db.Rci.Where(m => m.GordonID == null 
                                                                    && m.BuildingCode.Equals(buildingCode)
                                                                    && m.RoomNumber.Equals(apartmentNumber)));
            }
            db.Rci.RemoveRange(oldRcis);
            db.SaveChanges();

            var residentDictionary = new Dictionary<string, Dictionary<string,string>>();
            residentDictionary.Add(Credentials.APT_RES_1_USERNAME, new Dictionary<string, string>
            {
                {"password", Credentials.APT_RES_1_PASSWORD },
                { "id", Credentials.APT_RES_1_ID_NUMBER}
            });
            residentDictionary.Add(Credentials.APT_RES_2_USERNAME, new Dictionary<string, string>
            {
                {"password", Credentials.APT_RES_2_PASSWORD },
                { "id", Credentials.APT_RES_2_ID_NUMBER}
            });
            residentDictionary.Add(Credentials.APT_RES_3_USERNAME, new Dictionary<string, string>
            {
                {"password", Credentials.APT_RES_3_PASSWORD },
                { "id", Credentials.APT_RES_3_ID_NUMBER}
            });
            residentDictionary.Add(Credentials.APT_RES_4_USERNAME, new Dictionary<string, string>
            {
                {"password", Credentials.APT_RES_4_PASSWORD },
                { "id", Credentials.APT_RES_4_ID_NUMBER}
            });

            // Residents log in in sequence
            wd.Navigate().GoToUrl(Values.START_URL);
            foreach (var entry in residentDictionary)
            {
                new LoginPage(wd).LoginAs(entry.Key, entry.Value["password"]);
                new DashboardPage(wd).Logout();
            }

            // RA logs in
            var loginPage = new LoginPage(wd);
            var dashboard = loginPage.LoginAs(Credentials.APT_RA_USERNAME, Credentials.APT_RA_PASSWORD);

            var rciCards = dashboard.GetAllRciCards();
            
            // Assert
            foreach(var rciCard in rciCards)
            {
                Assert.IsTrue(rciCard.isCheckinRci());
                Assert.IsTrue(rciCard.isUnsigned());
            }
            dashboard.Logout();

            // Login again as each resident to sign.
            foreach(var entry in residentDictionary)
            {
                loginPage.LoginAs(entry.Key, entry.Value["password"]);
                var acct = db.Account.Where(m => m.ID_NUM.Equals(entry.Value["id"])).First();
                var resident_name = acct.firstname + " " + acct.lastname;
                
                // Sign personal rci.
                dashboard.SelectFirstRciWithName(resident_name)
                    .asRciCheckinPage()
                    .HitNextToSignatures()
                    .Sign(resident_name)
                    .SubmitSignature();

                dashboard.Logout();
            }            

         
            //login.EnterUsername(Credentials.DORM_RA_USERNAME);
            //login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            //login.SubmitCredentials();

            //rciCard = dashboard.GetRciCardWithName(resident_name);

            //// Assert
            //Assert.IsTrue(rciCard.isCheckinRci());
            //Assert.IsTrue(rciCard.isSignedByResident());
            //Assert.IsFalse(rciCard.isSignedByRA());

            //// RA signs
            //var checkinRci = dashboard.SelectFirstRciWithName(resident_name).asRciCheckinPage();
            //var user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RA_ID_NUMBER)).First();
            //var userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            //checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            //rciCard = dashboard.GetRciCardWithName(resident_name);

            //// Assert
            //Assert.IsTrue(rciCard.isCheckinRci());
            //Assert.IsTrue(rciCard.isSignedByResident());
            //Assert.IsTrue(rciCard.isSignedByRA());

            ////RD logs in
            //dashboard.Logout();
            //login.EnterUsername(Credentials.DORM_RD_USERNAME);
            //login.EnterUserPassword(Credentials.DORM_RD_PASSWORD);
            //login.SubmitCredentials();

            //checkinRci = dashboard.SelectFirstRciWithName(resident_name).asRciCheckinPage();

            //// RD signs
            //user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RD_ID_NUMBER)).First();
            //userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            //checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            //rciCard = dashboard.GetRciCardWithName(resident_name);

            //// Assert
            //Assert.IsTrue(rciCard.isCheckoutRci());
            //Assert.IsFalse(rciCard.isSignedByResident());
            //Assert.IsFalse(rciCard.isSignedByRA());
            //Assert.IsFalse(rciCard.isSignedByRD());


            //// Cleanup 
            //var usedRci = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
            //db.Rci.RemoveRange(usedRci);
            //db.SaveChanges();
            //wd.Quit();
        }



    }
}
