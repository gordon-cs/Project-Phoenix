//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using Phoenix.Models;
//using Phoenix.Tests.Pages;
//using Phoenix.Tests.TestUtilities;
//using System.Linq;

//namespace Phoenix.Tests.Tests
//{
//    [TestClass]
//    public class CheckoutFlowTests
//    {
//        private IWebDriver wd = new ChromeDriver();
//        private RCIContext db = new RCIContext();

//        /// <summary>
//        /// Go through the checkout flow for rcis.
//        /// Steps:
//        /// - Delete any old rcis the resident might have
//        /// - Quickly go through the checkin flow with no assertions to make the rci green.
//        /// - Log in as the RA
//        /// - Verify the rci created is green and has no signatures then logout
//        /// - Input Resident signature
//        /// - Verify that rci is green and has RES signature
//        /// - Input RA signature
//        /// - Verify that rci is green and has RES and RA signatures, then logout
//        /// - Login as RD
//        /// - Sign the rci as an RD
//        /// - Verify that rci is green and RES, RA and RD signature blocks
//        /// </summary>
//        [TestMethod]
//        public void CheckoutFlow_DormBuilding()
//        {
//            try
//            {
//                // Clear old rcis
//                var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
//                db.Rci.RemoveRange(oldRcis);
//                db.SaveChanges();

//                var resident_name = Methods.GetFullName(Credentials.DORM_RES_ID_NUMBER);
//                var ra_name = Methods.GetFullName(Credentials.DORM_RA_ID_NUMBER);
//                var rd_name = Methods.GetFullName(Credentials.DORM_RD_ID_NUMBER);

//                // START  Quick checkin flow -- pls don't hate me for doing this :))))
//                wd.Navigate().GoToUrl(Values.START_URL);
//                var rciId = new LoginPage(wd)
//                    .LoginAs(Credentials.DORM_RES_USERNAME, Credentials.DORM_RES_PASSWORD)
//                    .GetRciCardWithName(resident_name["firstname"] + " " + resident_name["lastname"])
//                    .GetId();

//                new DashboardPage(wd)
//                    .SelectRci(rciId)
//                    .asRciCheckinPage()
//                    .HitNextToSignatures()
//                    .Sign(resident_name["firstname"] + " " + resident_name["lastname"])
//                    .SubmitSignature()
//                    .Logout()
//                    .LoginAs(Credentials.DORM_RA_USERNAME, Credentials.DORM_RA_PASSWORD)
//                    .SelectRci(rciId)
//                    .asRciCheckinPage()
//                    .HitNextToSignatures()
//                    .Sign(ra_name["firstname"] + " " + ra_name["lastname"])
//                    .SubmitSignature()
//                    .Logout()
//                    .LoginAs(Credentials.DORM_RD_USERNAME, Credentials.DORM_RD_PASSWORD)
//                    .SelectRci(rciId)
//                    .asRciCheckinPage()
//                    .HitNextToSignatures()
//                    .Sign(rd_name["firstname"] + " " + rd_name["lastname"])
//                    .SubmitSignature()
//                    .Logout();
//                // END Quick checkin flow. 
//                //At this point, if everything went well the resident's rci will be green

//                wd.Navigate().GoToUrl(Values.START_URL);

//                // RA logs in
//                LoginPage login = new LoginPage(wd);
//                var dashboard = login.LoginAs(Credentials.DORM_RA_USERNAME, Credentials.DORM_RA_PASSWORD);

//                var rciCard = dashboard.GetRciCard(rciId);

//                // Assert
//                Assert.IsTrue(rciCard.isCheckoutRci());
//                Assert.IsTrue(rciCard.isUnsigned());
//                Assert.IsFalse(rciCard.isSignedByResident());
//                Assert.IsFalse(rciCard.isSignedByRA());


//                // RA signs for resident
//                dashboard.SelectRci(rciId)
//                    .asRciCheckoutPage()
//                    .HitNextToResidentSignature()
//                    .Sign(resident_name["firstname"] + " " + resident_name["lastname"])
//                    .HitNextToRASignature()
//                    .GoHomeToDashboard();

//                rciCard = dashboard.GetRciCard(rciId);

//                // Assert
//                Assert.IsTrue(rciCard.isCheckoutRci());
//                Assert.IsTrue(rciCard.isSignedByResident());
//                Assert.IsFalse(rciCard.isSignedByRA());

//                // RA signs
//                dashboard
//                    .SelectRci(rciId)
//                    .asRciCheckoutPage()
//                    .HitNextToResidentSignature()
//                    .HitNextToRASignature()
//                    .Sign(ra_name["firstname"] + " " + ra_name["lastname"])
//                    .SubmitSignature();

//                rciCard = dashboard.GetRciCard(rciId);

//                // Assert
//                Assert.IsTrue(rciCard.isCheckoutRci());
//                Assert.IsTrue(rciCard.isSignedByResident());
//                Assert.IsTrue(rciCard.isSignedByRA());

//                //RD logs in
//                dashboard.Logout();
//                login.LoginAs(Credentials.DORM_RD_USERNAME, Credentials.DORM_RD_PASSWORD);

//                // RD signs
//                dashboard
//                    .SelectRci(rciId)
//                    .asRciCheckoutPage()
//                    .HitNextToResidentSignature()
//                    .HitNextToRASignature()
//                    .HitNextToRDSignature()
//                    .Sign(Credentials.DORM_RD_USERNAME, Credentials.DORM_RD_PASSWORD)
//                    .SubmitSignature();

//                rciCard = dashboard.GetRciCard(rciId);

//                // Assert
//                Assert.IsTrue(rciCard.isCheckoutRci());
//                Assert.IsTrue(rciCard.isSignedByResident());
//                Assert.IsTrue(rciCard.isSignedByRA());
//                Assert.IsTrue(rciCard.isSignedByRD());


//                // Cleanup 
//                var usedRci = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
//                db.Rci.RemoveRange(usedRci);
//                db.SaveChanges();
//            }
//            finally
//            {
//                wd.Quit();
//            }
//        }
//    }
//}
