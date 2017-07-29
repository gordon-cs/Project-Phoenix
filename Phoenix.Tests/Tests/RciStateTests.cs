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
        /// - Create an rci directly using the db
        /// - Log in as the RA
        /// - Verify the rci created is blue and has no signatures
        /// - Sign the rci as a resident using the db 
        /// - Verify that rci is blue and has RES signature
        /// - Sign the rci as the RA
        /// - Verify that rci is blue and has RES and RA signatures
        /// - Login as RD
        /// - Sign the rci as an RD
        /// - Verify that rci is green and has no blocks.
        /// </summary>
        [TestMethod]
        public void CheckinFlow()
        {
            var loginService = new LoginService();

            var dorms = loginService.GetKingdom(Credentials.RA_ID_NUMBER);
            // Choose a random building code
            var dorm =  dorms[Methods.GetRandomInteger(0, dorms.Count )];
            // Choose a random room number
            var roomNumber = Methods.GetRandomInteger(0, 300).ToString() ;
            // Choose a random account 
            var randomAccount = db.Account.AsEnumerable().ElementAt(Methods.GetRandomInteger(0, 400));

            // Create a common area rci
            var newRci = new Rci
            {
                IsCurrent = true,
                BuildingCode = dorm,
                RoomNumber = roomNumber,
                SessionCode = "20701",
                GordonID = randomAccount.ID_NUM,
                CreationDate = DateTime.Now
            };
            db.Rci.Add(newRci);
            db.SaveChanges();

            // Get its id
            var rciID = newRci.RciID;

            wd.Navigate().GoToUrl(Values.START_URL);

            // RA logs in
            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.RA_USERNAME);
            login.EnterUserPassword(Credentials.RA_PASSWORD);
            // Submitting the credentials leads us to the dashboard.
            DashboardPage dashboard = login.SubmitCredentials();

            var rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isUnsigned());
            Assert.IsFalse(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());


            // Resident signs
            newRci.CheckinSigRes = DateTime.Now;
            newRci.LifeAndConductSigRes = DateTime.Now;
            db.SaveChanges();

            // Reload page
            wd.Navigate().GoToUrl(wd.Url);

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());

            // RA signs
            var checkinRci = dashboard.SelectRci(rciID).asRciCheckinPage();
            var user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.RA_ID_NUMBER)).First();
            var userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckinRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsTrue(rciCard.isSignedByRA());

            //RD logs in
            login = dashboard.Logout();
            login.EnterUsername(Credentials.RD_USERNAME);
            login.EnterUserPassword(Credentials.RD_PASSWORD);
            dashboard = login.SubmitCredentials();

            checkinRci = dashboard.SelectRci(rciID).asRciCheckinPage();

            // RD signs
            user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.RD_ID_NUMBER)).First();
            userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkinRci.HitNextToSignatures().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsFalse(rciCard.isSignedByResident());
            Assert.IsFalse(rciCard.isSignedByRA());
            Assert.IsFalse(rciCard.isSignedByRD());


            // Cleanup 
            db.Rci.Remove(newRci);
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
        public void CheckoutFlow()
        {
            var loginService = new LoginService();

            var dorms = loginService.GetKingdom(Credentials.RA_ID_NUMBER);
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
                CheckinSigRAGordonID = Credentials.RA_ID_NUMBER,
                CheckinSigRD = DateTime.Now,
                CheckinSigRDGordonID = Credentials.RD_ID_NUMBER
            };
            db.Rci.Add(newRci);
            db.SaveChanges();

            // Get its id
            var rciID = newRci.RciID;

            wd.Navigate().GoToUrl(Values.START_URL);

            // RA logs in
            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.RA_USERNAME);
            login.EnterUserPassword(Credentials.RA_PASSWORD);
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
            var user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.RA_ID_NUMBER)).First();
            var userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkoutRci.HitNextToResidentSignature().HitNextToRASignature().Sign(userSignature).SubmitSignature();

            rciCard = dashboard.GetRciCard(rciID);

            // Assert
            Assert.IsTrue(rciCard.isCheckoutRci());
            Assert.IsTrue(rciCard.isSignedByResident());
            Assert.IsTrue(rciCard.isSignedByRA());

            //RD logs in
            login = dashboard.Logout();
            login.EnterUsername(Credentials.RD_USERNAME);
            login.EnterUserPassword(Credentials.RD_PASSWORD);
            dashboard = login.SubmitCredentials();

            checkoutRci = dashboard.SelectRci(rciID).asRciCheckoutPage();

            // RD signs
            user = db.Account.Where(m => m.ID_NUM.Equals(Credentials.RD_ID_NUMBER)).First();
            userSignature = string.Format("{0} {1}", user.firstname, user.lastname);
            dashboard = checkoutRci.HitNextToResidentSignature()
                .HitNextToRASignature()
                .HitNextToRDSignature()
                .Sign(Credentials.RD_USERNAME, Credentials.RD_PASSWORD)
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
