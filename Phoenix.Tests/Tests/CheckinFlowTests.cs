using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Phoenix.Models;
using Phoenix.Tests.TestUtilities;
using Phoenix.Tests.Pages;
using System.Linq;
using System.Collections.Generic;
using Phoenix.Tests.Models;

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
            try
            {
                // Clear old rcis
                var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
                db.Rci.RemoveRange(oldRcis);
                db.SaveChanges();

                wd.Navigate().GoToUrl(Values.START_URL);

                var resident_name = Methods.GetFullName(Credentials.DORM_RES_ID_NUMBER);

                // Resident logs in
                LoginPage login = new LoginPage(wd);
                var dashboard = login.LoginAs(Credentials.DORM_RES_USERNAME, Credentials.DORM_RES_PASSWORD);
                var rciId = dashboard.GetRciCardWithName(resident_name["firstname"] + " " + resident_name["lastname"])
                    .GetId();

                dashboard.Logout();

                // RA logs in
                login.LoginAs(Credentials.DORM_RA_USERNAME, Credentials.DORM_RA_PASSWORD);


                var rciCard = dashboard.GetRciCard(rciId);

                // Assert
                Assert.IsTrue(rciCard.isCheckinRci());
                Assert.IsTrue(rciCard.isUnsigned());
                Assert.IsFalse(rciCard.isSignedByResident());
                Assert.IsFalse(rciCard.isSignedByRA());

                // Login again as resident.
                dashboard.Logout();
                login.LoginAs(Credentials.DORM_RES_USERNAME, Credentials.DORM_RES_PASSWORD);

                GenericRciPage rci = dashboard.SelectRci(rciId);
                rci.asRciCheckinPage()
                    .HitNextToSignatures()
                    .Sign(resident_name["firstname"] + " " + resident_name["lastname"])
                    .SubmitSignature();
                dashboard.Logout();

                login.LoginAs(Credentials.DORM_RA_USERNAME, Credentials.DORM_RA_PASSWORD);

                rciCard = dashboard.GetRciCard(rciId);

                // Assert
                Assert.IsTrue(rciCard.isCheckinRci());
                Assert.IsTrue(rciCard.isSignedByResident());
                Assert.IsFalse(rciCard.isSignedByRA());

                // RA signs
                var checkinRci = dashboard.SelectRci(rciId)
                    .asRciCheckinPage();

                // The fullname is also the signature
                var ra_name = Methods.GetFullName(Credentials.DORM_RA_ID_NUMBER);

                checkinRci.HitNextToSignatures().Sign(ra_name["firstname"] + " " + ra_name["lastname"]).SubmitSignature();

                rciCard = dashboard.GetRciCard(rciId);

                // Assert
                Assert.IsTrue(rciCard.isCheckinRci());
                Assert.IsTrue(rciCard.isSignedByResident());
                Assert.IsTrue(rciCard.isSignedByRA());

                //RD logs in
                dashboard.Logout();

                login.LoginAs(Credentials.DORM_RD_USERNAME, Credentials.DORM_RD_PASSWORD);

                checkinRci = dashboard.SelectRci(rciId)
                    .asRciCheckinPage();

                // RD signs
                var rd_name = Methods.GetFullName(Credentials.DORM_RD_ID_NUMBER);
                checkinRci.HitNextToSignatures()
                    .Sign(rd_name["firstname"] + " " + rd_name["lastname"])
                    .SubmitSignature();

                rciCard = dashboard.GetRciCard(rciId);

                // Assert
                Assert.IsTrue(rciCard.isCheckoutRci());
                Assert.IsFalse(rciCard.isSignedByResident());
                Assert.IsFalse(rciCard.isSignedByRA());
                Assert.IsFalse(rciCard.isSignedByRD());


                // Cleanup 
                var usedRci = db.Rci.Where(m => m.GordonID.Equals(Credentials.DORM_RES_ID_NUMBER));
                db.Rci.RemoveRange(usedRci);
                db.SaveChanges();
            }
            finally
            {
                wd.Quit();
            }
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
            try
            {
                // Clear old rcis
                var oldRcis = db.Rci.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER)
                                                                || m.GordonID.Equals(Credentials.APT_RES_2_ID_NUMBER)
                                                                || m.GordonID.Equals(Credentials.APT_RES_3_ID_NUMBER)
                                                                || m.GordonID.Equals(Credentials.APT_RES_4_ID_NUMBER));
                // Clear the common area rci.
                if (oldRcis.Any())
                {
                    var buildingCode = oldRcis.First().BuildingCode;
                    var roomNumber = oldRcis.First().RoomNumber;
                    var apartmentNumber = roomNumber.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });

                    oldRcis.Concat(db.Rci.Where(m => m.GordonID == null
                                                                        && m.BuildingCode.Equals(buildingCode)
                                                                        && m.RoomNumber.Equals(apartmentNumber)));
                }
                // Clear old common area signatures
                var oldSignatures = db.CommonAreaRciSignature.Where(m => m.GordonID.Equals(Credentials.APT_RES_1_ID_NUMBER)
                                                               || m.GordonID.Equals(Credentials.APT_RES_2_ID_NUMBER)
                                                               || m.GordonID.Equals(Credentials.APT_RES_3_ID_NUMBER)
                                                               || m.GordonID.Equals(Credentials.APT_RES_4_ID_NUMBER));

                db.Rci.RemoveRange(oldRcis);
                db.CommonAreaRciSignature.RemoveRange(oldSignatures);
                db.SaveChanges();

                // Create a dictionary with important values for each resident.
                // This lets us use a for loop when we want to do the same action for each resident.
                var residentDictionary = new Dictionary<string, Dictionary<string, string>>();
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
                    var resident_name = Methods.GetFullName(entry.Value["id"]);

                    var rciId = new LoginPage(wd)
                        .LoginAs(entry.Key, entry.Value["password"])
                        .GetRciCardWithName(resident_name["firstname"] + " " + resident_name["lastname"])
                        .GetId();

                    // Get the RciId of each resident.
                    entry.Value.Add("rciId", rciId.ToString());

                    new DashboardPage(wd).Logout();
                }

                // RA logs in
                var loginPage = new LoginPage(wd);
                var dashboard = loginPage.LoginAs(Credentials.APT_RA_USERNAME, Credentials.APT_RA_PASSWORD);

                var rciCards = new List<RciCard>();

                foreach (var entry in residentDictionary)
                {
                    rciCards.Add(dashboard.GetRciCard(int.Parse(entry.Value["rciId"])));
                }

                // Assert
                foreach (var rciCard in rciCards)
                {
                    Assert.IsTrue(rciCard.isCheckinRci());
                    Assert.IsTrue(rciCard.isUnsigned());
                }

                dashboard.Logout();

                // Login again as each resident to sign.
                foreach (var entry in residentDictionary)
                {
                    loginPage.LoginAs(entry.Key, entry.Value["password"]);

                    var gordonID = entry.Value["id"];
                    var resident_name = Methods.GetFullName(gordonID);
                    var rciId = entry.Value["rciId"];

                    // Sign personal rci.
                    dashboard.SelectRci(int.Parse(rciId))
                        .asRciCheckinPage()
                        .HitNextToSignatures()
                        .Sign(resident_name["firstname"] + " " + resident_name["lastname"])
                        .SubmitSignature();

                    var roomID = Methods.GetRoomID(gordonID);

                    // Sign common area rci
                    dashboard.SelectCommonAreaRci(roomID["building"] + " " + roomID["room"].TrimEnd(new char[] { 'A', 'B', 'C', 'D' }))
                        .asRciCheckinPage()
                        .HitNextToSignatures()
                        .Sign(resident_name["firstname"] + " " + resident_name["lastname"])
                        .SubmitSignature();
                    dashboard.Logout();
                }

                // RA logs in once more
                loginPage.LoginAs(Credentials.APT_RA_USERNAME, Credentials.APT_RA_PASSWORD);

                // Assert then sign each rci.
                foreach (var entry in residentDictionary)
                {
                    var rciCard = dashboard.GetRciCard(int.Parse(entry.Value["rciId"]));

                    // Assert 
                    Assert.IsTrue(rciCard.isCheckinRci());
                    Assert.IsTrue(rciCard.isSignedByResident());
                    Assert.IsFalse(rciCard.isSignedByRA());
                    Assert.IsFalse(rciCard.isSignedByRD());

                    var ra_name = Methods.GetFullName(Credentials.APT_RA_ID_NUMBER);

                    // Sign
                    var rciPage = rciCard.Click();
                    rciPage.asRciCheckinPage()
                        .HitNextToSignatures()
                        .Sign(ra_name["firstname"] + " " + ra_name["lastname"])
                        .SubmitSignature();
                }

                dashboard.Logout();

                // Loging As RD To Complete Stuff
                loginPage
                    .LoginAs(Credentials.APT_RD_USERNAME, Credentials.APT_RD_PASSWORD);

                // Assert
                foreach (var entry in residentDictionary)
                {
                    var rciCard = dashboard.GetRciCard(int.Parse(entry.Value["rciId"]));

                    Assert.IsTrue(rciCard.isCheckinRci());
                    Assert.IsTrue(rciCard.isSignedByResident());
                    Assert.IsTrue(rciCard.isSignedByRA());
                    Assert.IsFalse(rciCard.isSignedByRD());

                    var rciPage = rciCard.Click();
                    rciPage.asRciCheckinPage()
                        .HitNextToSignatures()
                        .ToggleQueueForSigningCheckbox()
                        .SubmitSignature();
                }
            }
            finally
            {
                wd.Quit();
            }
        }
    }
}
