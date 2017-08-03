using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phoenix.Services;
using Phoenix.Tests.TestUtilities;
using Phoenix.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Phoenix.Tests.Pages;
using System.Linq;

namespace Phoenix.Tests.Tests
{
    [TestClass]
    public class RciSignatureTests
    {
        private IWebDriver wd = new ChromeDriver();
        private RCIContext db = new RCIContext();
        /// <summary>
        /// Verify that an  RA cannot sign an Rci before the resident does so.
        /// Steps:
        /// 1- Create a random Rci
        /// 2- Login as the RA and try to sign it. This should fail.
        /// 2- Verify that we get the message about not being able to sign before residents do.
        /// </summary>
        [TestMethod]
        public void RciSignature_Test_1()
        {
            var loginService = new LoginService();

            var dorms = loginService.GetKingdom(Credentials.DORM_RA_ID_NUMBER);
            // Choose a random building code
            var dorm = dorms[Methods.GetRandomInteger(0, dorms.Count)];
            // Choose a random room number
            var roomNumber = Methods.GetRandomInteger(0, 300).ToString();

            var randomAccount = db.Account.AsEnumerable().ElementAt(Methods.GetRandomInteger(0, 400));

            // Create an rci
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

            var rciID = newRci.RciID;

            wd.Navigate().GoToUrl(Values.START_URL);

            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            DashboardPage dashboard = login.SubmitCredentials();
            RciCheckinPage rci = dashboard.SelectRci(rciID).asRciCheckinPage();

            bool canSign = true;
            try
            {
                rci.HitNextToSignatures().Sign("").SubmitSignature();
            }
            catch (IllegalStateException e)
            {
                // There was no input box to sign.
                canSign = false;
            }

            Assert.IsFalse(canSign, "RA could sign even though the resident had not yet signed.");
            Assert.IsTrue(rci.GetSignaturePagePopupText().Contains("The resident hasn't signed yet. Please make sure the resident has signed before signing."));


            // Cleanup
            db.Rci.Remove(newRci);
            db.SaveChanges();
            wd.Quit();
        }

        /// <summary>
        /// Verify that an RD cannot sign an Rci before the resident does so.
        /// Steps:
        /// 1- Create a random Rci
        /// 2- Login as the RD and try to sign it. This should fail.
        /// 2- Verify that we get the message about not being able to sign before residents do.
        /// </summary>
        [TestMethod]
        public void RciSignature_Test_2()
        {
            var loginService = new LoginService();

            var dorms = loginService.GetKingdom(Credentials.DORM_RD_ID_NUMBER);
            // Choose a random building code
            var dorm = dorms[Methods.GetRandomInteger(0, dorms.Count)];
            // Choose a random room number
            var roomNumber = Methods.GetRandomInteger(0, 300).ToString();

            var randomAccount = db.Account.AsEnumerable().ElementAt(Methods.GetRandomInteger(0, 400));

            // Create an  rci
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

            var rciID = newRci.RciID;

            wd.Navigate().GoToUrl(Values.START_URL);

            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.DORM_RD_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RD_PASSWORD);
            DashboardPage dashboard = login.SubmitCredentials();
            RciCheckinPage rci = dashboard.SelectRci(rciID).asRciCheckinPage();

            bool canSign = true;
            try
            {
                rci.HitNextToSignatures().Sign("").SubmitSignature();
            }
            catch (IllegalStateException e)
            {
                // There was no input box to sign.
                canSign = false;
            }

            Assert.IsFalse(canSign, "RD could sign even though the resident had not yet signed.");
            Assert.IsTrue(rci.GetSignaturePagePopupText().Contains("The resident hasn't signed yet. Please make sure the resident and RA have signed before signing."));

            // Cleanup
            db.Rci.Remove(newRci);
            db.SaveChanges();
            wd.Quit();
        }

        /// <summary>
        /// Verify that an RD cannot sign an Rci before the RA does so.
        /// Steps:
        /// 1- Create a random Rci
        /// 2- Have the resident sign it. (By directly altering the database since we don't have a normal test user)
        /// 3- Login as the RD and try to sign it. This should fail.
        /// 4- Verify that we get the message about not being able to sign before the RA does so.
        /// </summary>
        [TestMethod]
        public void RciSignature_Test_3()
        {
            var loginService = new LoginService();

            var dorms = loginService.GetKingdom(Credentials.DORM_RD_ID_NUMBER);
            // Choose a random building code
            var dorm = dorms[Methods.GetRandomInteger(0, dorms.Count)];
            // Choose a random room number
            var roomNumber = Methods.GetRandomInteger(0, 300).ToString();

            var randomAccount = db.Account.AsEnumerable().ElementAt(Methods.GetRandomInteger(0, 400));

            // Create  an rci signed by the resident
            var newRci = new Rci
            {
                IsCurrent = true,
                BuildingCode = dorm,
                RoomNumber = roomNumber,
                SessionCode = "20701",
                GordonID = randomAccount.ID_NUM,
                CreationDate = DateTime.Now,
                LifeAndConductSigRes = DateTime.Now,
                CheckinSigRes = DateTime.Now
            };
            db.Rci.Add(newRci);
            db.SaveChanges();

            var rciID = newRci.RciID;

            wd.Navigate().GoToUrl(Values.START_URL);

            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.DORM_RD_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RD_PASSWORD);
            DashboardPage dashboard = login.SubmitCredentials();
            RciCheckinPage rci = dashboard.SelectRci(rciID).asRciCheckinPage();

            bool canSign = true;
            try
            {
                rci.HitNextToSignatures().Sign("").SubmitSignature();
            }
            catch (IllegalStateException e)
            {
                // There was no input box to sign.
                canSign = false;
            }

            Assert.IsFalse(canSign, "RD could sign even though the RA had not yet signed.");
            Assert.IsTrue(rci.GetSignaturePagePopupText().Contains("The RA/AC hasn't signed yet. Please make sure the RA/AC has signed before signing."));

            // Cleanup
            db.Rci.Remove(newRci);
            db.SaveChanges();
            wd.Quit();
        }
        
        /// <summary>
        /// Verify that an RA can sign an Rci both as herself/himself and as an RA.
        /// Steps:
        /// 1- Login as an RA.
        /// 2- Verify that there is a new/empty rci already created
        /// 3- Verify that it isn't signed.
        /// 4- Sign it once. 
        /// 5- Verify that it is now signed as both RA and Resident.
        /// 6- From the dashboard, select the rci a second time and verify that we are now directed to the review page.
        /// </summary>
        [TestMethod]
        public void RciSignature_Test_4()
        {
            var rci = db.Rci.Where(r => r.GordonID == Credentials.DORM_RA_ID_NUMBER && r.IsCurrent == true).First();


            wd.Navigate().GoToUrl(Values.START_URL);

            LoginPage login = new LoginPage(wd);
            login.EnterUsername(Credentials.DORM_RA_USERNAME);
            login.EnterUserPassword(Credentials.DORM_RA_PASSWORD);
            DashboardPage dashboard = login.SubmitCredentials();
            RciCheckinPage personalRci = dashboard.SelectRci(rci.RciID).asRciCheckinPage();

            var account = db.Account.Where(a => a.ID_NUM == Credentials.DORM_RA_ID_NUMBER).First();
            var signature = string.Format("{0} {1}", account.firstname, account.lastname);

            dashboard = personalRci.HitNextToSignatures().Sign(signature).SubmitSignature();

            var rciCard = dashboard.GetRciCard(rci.RciID);

            // Assert
            Assert.IsTrue(rciCard.isSignedByResident(), "The RES signature block didn't show up");
            Assert.IsTrue(rciCard.isSignedByRA(), "The RA signature block didn't show up");
            Assert.IsTrue(dashboard.SelectRci(rci.RciID).asRciCheckinPage().isReviewPage);

            // Cleanup
            db.Rci.Remove(rci);
            db.SaveChanges();
            wd.Quit();
        }
    }
}
