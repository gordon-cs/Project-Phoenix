using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Phoenix.Services
{
    public class RciCheckoutService
    {
        private RCIContext db;
        public RciCheckoutService()
        {
            db = new RCIContext();
        }

        /// <summary>
        /// Fetch the rci straight from the db without mapping it to a model.
        /// </summary>
        public Rci GetBareRciByID(int id)
        {
            return db.Rci.Find(id);
        }
        /// <summary>
        /// Get a generic checkout rci view model.
        /// The RA and RD views use this since they don't need the other information
        /// </summary>
        public GenericCheckoutViewModel GetGenericCheckoutRciByID(int id)
        {
            var query =
                from r in db.Rci
                where r.RciID == id
                select new GenericCheckoutViewModel()
                {
                    RciID = r.RciID,
                    BuildingCode = r.BuildingCode,
                    RoomNumber = r.RoomNumber,
                    CheckoutSigRes = r.CheckoutSigRes,
                    CheckoutSigRA = r.CheckoutSigRA,
                    CheckoutSigRD = r.CheckoutSigRD,
                    CheckoutSigRAGordonID = r.CheckoutSigRAGordonID,
                    CheckoutSigRDGordonID = r.CheckoutSigRDGordonID,
                    CheckoutSigRAName =
                                        (from acct in db.Account
                                         where acct.ID_NUM.Equals(r.CheckoutSigRAGordonID)
                                         select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    CheckoutSigRDName =
                                         (from acct in db.Account
                                          where acct.ID_NUM.Equals(r.CheckoutSigRDGordonID)
                                          select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    RciComponent = r.RciComponent

                };

            return query.FirstOrDefault();
        }
        /// <summary>
        /// Get the rci for a person by RciID
        /// </summary>
        public CheckoutIndividualRoomRciViewModel GetIndividualRoomRciByID(int id)
        {
            var query =
                from r in db.Rci
                join a in db.Account on r.GordonID equals a.ID_NUM
                where r.RciID == id
                select new CheckoutIndividualRoomRciViewModel()
                {
                    RciID = r.RciID,
                    GordonID = r.GordonID,
                    FirstName = a.firstname,
                    LastName = a.lastname,
                    BuildingCode = r.BuildingCode,
                    RoomNumber = r.RoomNumber,
                    RciComponent = r.RciComponent,
                    CheckoutSigRes = r.CheckoutSigRes,
                    CheckoutSigRA = r.CheckoutSigRA,
                    CheckoutSigRD = r.CheckoutSigRD,
                    CheckoutSigRAGordonID = r.CheckoutSigRAGordonID,
                    CheckoutSigRDGordonID = r.CheckoutSigRDGordonID,
                    CheckoutSigRAName =
                                        (from acct in db.Account
                                         where acct.ID_NUM.Equals(r.CheckoutSigRAGordonID)
                                         select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    CheckoutSigRDName =
                                         (from acct in db.Account
                                          where acct.ID_NUM.Equals(r.CheckoutSigRDGordonID)
                                          select acct.firstname + " " + acct.lastname).FirstOrDefault()

                };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get the rci for a common area by ID
        /// </summary>
        public CheckoutCommonAreaRciViewModel GetCommonAreaRciByID(int id)
        {
            var currentSession = new DashboardService().GetCurrentSession();

            var query =
                from rci in db.Rci
                where rci.RciID == id
                select new CheckoutCommonAreaRciViewModel
                {
                    RciID = rci.RciID,
                    BuildingCode = rci.BuildingCode,
                    RoomNumber = rci.RoomNumber,
                    RciComponent = rci.RciComponent,
                    CommonAreaMember =
                                        (from rm in db.RoomAssign
                                         join acct in db.Account
                                         on rm.ID_NUM.ToString() equals acct.ID_NUM
                                         where rm.SESS_CDE.Trim() == currentSession
                                         && rm.BLDG_CDE.Trim() == rci.BuildingCode
                                         && rm.ROOM_CDE.Trim().Contains(rci.RoomNumber)
                                         select new CommonAreaMember
                                         {
                                             GordonID = acct.ID_NUM,
                                             FirstName = acct.firstname,
                                             LastName = acct.lastname,
                                             HasSignedCommonAreaRci =
                                                            ((from sigs in db.CommonAreaRciSignature
                                                              where sigs.GordonID == acct.ID_NUM
                                                              && sigs.RciID == rci.RciID
                                                              && sigs.SignatureType == "CHECKOUT"
                                                              select sigs).Any() == true ? true : false),
                                             Signature =
                                                             ((from sigs in db.CommonAreaRciSignature
                                                               where sigs.GordonID == acct.ID_NUM
                                                               && sigs.RciID == rci.RciID
                                                               && sigs.SignatureType == "CHECKOUT"
                                                               select sigs).FirstOrDefault().Signature)
                                         }).ToList(),
                    CheckoutSigRes = rci.CheckoutSigRes,
                    CheckoutSigRA = rci.CheckoutSigRA,
                    CheckoutSigRD = rci.CheckoutSigRD,
                    CheckoutSigRAGordonID = rci.CheckoutSigRAGordonID,
                    CheckoutSigRDGordonID = rci.CheckoutSigRDGordonID,
                    CheckoutSigRAName =
                                        (from acct in db.Account
                                         where acct.ID_NUM.Equals(rci.CheckoutSigRAGordonID)
                                         select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    CheckoutSigRDName =
                                         (from acct in db.Account
                                          where acct.ID_NUM.Equals(rci.CheckoutSigRDGordonID)
                                          select acct.firstname + " " + acct.lastname).FirstOrDefault()

                };

            return query.FirstOrDefault();

        }

        /// <summary>
        /// Add a fine record to the database
        /// </summary>
        public int AddFine(RciNewFineViewModel newFine)
        {
            var obj = new Fine { RciComponentID = newFine.ComponentID, Reason = newFine.FineReason, FineAmount = newFine.FineAmount, GordonID = newFine.FineOwner };
            db.Fine.Add(obj);
            db.SaveChanges();
            return obj.FineID;
        }

        /// <summary>
        /// Delete a fine from the database
        /// </summary>
        public void RemoveFine(int fineID)
        {
            var fine = db.Fine.Find(fineID);
            db.Fine.Remove(fine);
            db.SaveChanges();
        }

        /// <summary>
        /// Let the indicated resident sign the rci. This signature is represented by adding a record to the
        /// CommonAreaRciSignature table.
        /// </summary>
        public void CheckoutCommonAreaMemberSignRci(int rciID, string gordonID)
        {
            var signature = new CommonAreaRciSignature
            {
                RciID = rciID,
                GordonID = gordonID,
                Signature = DateTime.Now,
                SignatureType = "CHECKOUT"
            };
            db.CommonAreaRciSignature.Add(signature);
            db.SaveChanges();
        }
        /// <summary>
        /// Sign the resident portion of the rci during the checkout process
        /// </summary>
        public void CheckoutResidentSignRci(int rciID)
        {
            var rci = db.Rci.Find(rciID);

            rci.CheckoutSigRes = System.DateTime.Today;

            db.SaveChanges();

        }

        /// <summary>
        /// Sign the RA portion of the rci during the checkout process
        /// </summary>
        public void CheckoutRASignRci(int rciID, string raGordonID)
        {
            var rci = db.Rci.Find(rciID);

            rci.CheckoutSigRA = System.DateTime.Today;
            rci.CheckoutSigRAGordonID = raGordonID;

            db.SaveChanges();

        }

        /// <summary>
        /// Sign the RD portion of the rci during the checkout process and make the rci non-current.
        /// </summary>
        public void CheckoutRDSignRci(int rciID, string rdGordonID)
        {
            var rci = db.Rci.Find(rciID);

            rci.CheckoutSigRD = System.DateTime.Today;
            rci.CheckoutSigRDGordonID = rdGordonID;

            db.SaveChanges();

        }

        /// <summary>
        /// Send the fine email(s) associated with the corresponding rci
        /// </summary>
        /// <param name="rciID"></param>
        public void SendFineEmail(int rciID, string emailAddress, string password)
        {
            var rci = db.Rci.Find(rciID);
            var fineEmailDictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var component in rci.RciComponent)
            {
                foreach (var fine in component.Fine)
                {
                    if (fineEmailDictionary.ContainsKey(fine.GordonID))
                    {
                        fineEmailDictionary[fine.GordonID]["body"] += "<p>" + component.RciComponentName + " - " + fine.Reason + ": $" + fine.FineAmount + "</p>";
                        var total = decimal.Parse(fineEmailDictionary[fine.GordonID]["total"]);
                        total = total + fine.FineAmount;
                        fineEmailDictionary[fine.GordonID]["total"] = total.ToString();
                    }
                    else
                    {

                        var newFineEmailContents = new Dictionary<string, string>
                        {
                            {"body",  "<p>" + component.RciComponentName + " - " + fine.Reason + ": $" + fine.FineAmount + "</p>" },
                            {"total", fine.FineAmount.ToString() }
                        };
                        fineEmailDictionary.Add(fine.GordonID, newFineEmailContents);

                    }
                }
            }

            foreach (KeyValuePair<string, Dictionary<string, string>> entry in fineEmailDictionary)
            {
                var message = new MailMessage();
                var recepientAccount = db.Account.Where(r => r.ID_NUM.Equals(entry.Key)).FirstOrDefault();
                var to = recepientAccount.email;
                var from = emailAddress;
                var today = DateTime.Now.ToLongDateString();
                var recepientName = recepientAccount.firstname; 
                message.To.Add(new MailAddress(to));
                message.From = new MailAddress(from);
                message.Subject = "Checkout Fines - " + rci.BuildingCode + " " + rci.RoomNumber;
                message.Body = string.Format(Properties.Resources.FINE_EMAIL, 
                    today, 
                    recepientName, 
                    entry.Value["body"], 
                    "$" + entry.Value["total"]);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = emailAddress,
                        Password = password
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.office365.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
            }
        }

        public void WorkRequestDamages(List<string> workRequests, string username, string password, int rciID)
        {
            // Go.gordon.edu can be accessed using Basic authentication. I found this out via trial and error xD
            string authenticationInfo = username + ":" + password;
            authenticationInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authenticationInfo));

            var acct = db.Account.Where(x => x.AD_Username.Equals(username)).FirstOrDefault();
            var rci = db.Rci.Find(rciID);
            var buildingName = db.Room.Where(x => x.BLDG_CDE.Equals(rci.BuildingCode)).First().BUILDING_DESC;

            // Set up the Http client
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.GO_GORDON_URL);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationInfo);
                client.DefaultRequestHeaders.ExpectContinue = false;
                client.DefaultRequestHeaders.ConnectionClose = false;

                foreach (var request in workRequests)
                {
                    PostWorkRequest(client, request, acct.AD_Username, buildingName, rci.RoomNumber, "9789272300", acct.firstname, acct.lastname, acct.ID_NUM);
                }
            }
        }

        public HttpStatusCode PostWorkRequest(HttpClient client, 
                                                                        string workRequest, 
                                                                        string ADUsername, 
                                                                        string fullBuildingName, 
                                                                        string roomNumber,
                                                                        string phoneNumber, 
                                                                        string firstname,
                                                                        string lastname,
                                                                        string gordonID)
        {
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("submitted_by", ADUsername));
            data.Add(new KeyValuePair<string, string>("ad_username", ADUsername));
            data.Add(new KeyValuePair<string, string>("building", fullBuildingName));
            data.Add(new KeyValuePair<string, string>("location", "Room " + roomNumber));
            data.Add(new KeyValuePair<string, string>("strdescription", workRequest));
            data.Add(new KeyValuePair<string, string>("phone", phoneNumber));
            data.Add(new KeyValuePair<string, string>("fname", firstname));
            data.Add(new KeyValuePair<string, string>("lname", lastname));
            data.Add(new KeyValuePair<string, string>("gordon_id", gordonID));
            data.Add(new KeyValuePair<string, string>("m_date_needed", "1"));
            data.Add(new KeyValuePair<string, string>("d_date_needed", "1"));
            data.Add(new KeyValuePair<string, string>("y_date_needed", "1900"));
            data.Add(new KeyValuePair<string, string>("hr_date_needed", "00"));
            data.Add(new KeyValuePair<string, string>("mm_date_needed", "00"));
            data.Add(new KeyValuePair<string, string>("tt_date_needed", "AM"));
            data.Add(new KeyValuePair<string, string>("m_date_to", "1"));
            data.Add(new KeyValuePair<string, string>("d_date_to", "1"));
            data.Add(new KeyValuePair<string, string>("y_date_to", "1900"));
            data.Add(new KeyValuePair<string, string>("hr_date_to", "00"));
            data.Add(new KeyValuePair<string, string>("mm_date_to", "00"));
            data.Add(new KeyValuePair<string, string>("tt_date_to", "AM"));

            var content = new FormUrlEncodedContent(data);

            var response = client.PostAsync(Constants.WORK_REQUEST_ENDPOINT, content).Result;
            var statusCode = response.StatusCode;

            Debug.WriteLine("This task yeilded a " + statusCode + " status code");
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);

            return statusCode;
        }


        public IEnumerable<string> GetCommonRooms(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci.RciComponent.GroupBy(x => x.RciComponentDescription).Select(x => x.First()).Select(x => x.RciComponentDescription);
        }

    }
}