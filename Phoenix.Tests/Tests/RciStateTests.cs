using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phoenix.Models;
using Phoenix.Services;
using Phoenix.Tests.TestUtilities;
using Phoenix.Tests.Pages;
using System.Linq;

namespace Phoenix.Tests.Tests
{
    [TestClass]
    public class RciStateTests
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
        /// - Sign the rci, the logout.
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

            login = dashboard.Logout();

            // RA logs in
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            // Submitting the credentials leads us to the dashboard.
            dashboard = login.SubmitCredentials();

            var res_acct = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RES_ID_NUMBER)).First();
            var resident_name = res_acct.firstname + " " + res_acct.lastname;
            var rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isUnsigned());
            Assert.IsFalse(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());

            // Login again as resident.
            login = dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RES_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RES_PASSWORD);
            dashboard = login.SubmitCredentials();

            GenericRciPage rci = dashboard.SelectFirstRciWithName(resident_name);
            dashboard = rci.asRciCheckinPage().HitNextToSignatures().Sign(resident_name).SubmitSignature();

            login = dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            dashboard = login.SubmitCredentials();

            rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());

            // RA signs
            var checkinRci = dashboard.SelectFirstRciWithName(resident_name).asRciCheckinPage();
            var user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RA_ID_NUMBER)).First();
            var userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCardWithName(resident_name);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsTrue(rciCard.isSignedByRA());

            //RD logs in
            login = dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RD_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RD_PASSWORD);
            dashboard = login.SubmitCredentials();

            checkinRci = dashboard.SelectFirstRciWithName(resident_name).asRciCheckinPage();

            // RD signs
            user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RD_ID_NUMBER)).First();
            userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

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
        /// Go through the checkout flow for rcis.
        /// Steps:
        /// - Create a checkout rci directly using the db
        /// - Log in as the RA
        /// - Verify the rci created is green and has no signatures
        /// - Sign the rci as a resident using the db 
        /// - Verify that rci is green and has RES signature
        /// - Sign the rci as the RA
        /// - Verify that rci is green and has RES and RA signatures
        /// - Login as RD
        /// - Sign the rci as an RD
        /// - Verify that rci is green and RES, RA and RD signature blocks
        /// </summary>
        [TestMethod]
        public void CheckoutFlow_DormBuilding()
        {
            var loginService = new LoginService();

            var dorms = loginService.GetKingdom(Credentials.DORM_RA_ID_NUMBER);
            // Choose a random building code
            var dorm = dorms[Methods.GetRandomInteger(0, dorms.Count)];
            // Choose a random room number
            var roomNumber = Methods.GetRandomInteger(0, 300).ToString();
            // Choose a random account
            var randomAccount = db.Account.AsEnumerable().ElementAt(Methods.GetRandomInteger(0, 400));

            // Create an rci ready for checkout
            var newRci = new Rci
            {
                IsCurrent = true,
                BuildingCode = dorm,
                RoomNumber = roomNumber,
                SessionCode = "20701",
                GordonID =  randomAccount.ID_NUM,
                CreationDate = DateTime.Now,
                CheckinSigRes = DateTime.Now,
                LifeAndConductSigRes = DateTime.Now,
                CheckinSigRA = DateTime.Now,
                CheckinSigRAGordonID = Credentials.DORM_RA_ID_NUMBER,
                CheckinSigRD = DateTime.Now,
                CheckinSigRDGordonID = Credentials.DORM_RD_ID_NUMBER
            };
            db.Rci.Add(newRci);
            db.SaveChanges();

            // Get its id
            var rciID = newRci.RciID;

            wd.Navigate().GoToUrl(Values.START_URL);

            // RA logs in
            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            // Submitting the credentials leads us to the dashboard.
            DashboardPage dashboard = login.SubmitCredentials();

            var rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsTrue(rciCard.isUnsigned());
            Assert.IsFalse(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());


            // Resident signs
            newRci.CheckoutSigRes = DateTime.Now;
            db.SaveChanges();

            // Reload page
            wd.Navigate().GoToUrl(wd.Url);

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());

            // RA signs
            var checkoutRci = dashboard.SelectRci(rciID).asRciCheckoutPage();
            var user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RA_ID_NUMBER)).First();
            var userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkoutRci.HitNextToResidentSignature().HitNextToRASignature().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsTrue(rciCard.isSignedByRA());

            //RD logs in
            login = dashboard.Logout();
            login.EnterUsername(Credentials.DORM_RD_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RD_PASSWORD);
            dashboard = login.SubmitCredentials();

            checkoutRci = dashboard.SelectRci(rciID).asRciCheckoutPage();

            // RD signs
            user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.DORM_RD_ID_NUMBER)).First();
            userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkoutRci.HitNextToResidentSignature()
                .HitNextToRASignature()
                .HitNextToRDSignature()
                .Sign(Credentials.DORM_RD_USERNAME, Credentials.DORM_RD_PASSWORD)
                .SubmitSignature();

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsTrue(rciCard.isSignedByRA());
            Assert.IsTrue(rciCard.isSignedByRD());


            // Cleanup 
            db.Rci.Remove(newRci);
            db.SaveChanges();
            wd.Quit();
        }



    }
}
