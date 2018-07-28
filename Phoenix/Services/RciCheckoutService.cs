using Phoenix.DapperDal;
using Phoenix.DapperDal.Types;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;

namespace Phoenix.Services
{
    public class RciCheckoutService : IRciCheckoutService
    {
        private IDatabaseDal Dal { get; set; }
        
        private IDashboardService DashboardService { get; set; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RciCheckoutService(IDatabaseDal dal, IDashboardService dashboardService)
        {
            this.Dal = dal;

            this.DashboardService = dashboardService;
        }

        /// <summary>
        /// Fetch the rci straight from the db without mapping it to a model.
        /// </summary>
        public FullRciViewModel GetRciById(int id)
        {
            try
            {
                return new FullRciViewModel(this.Dal.FetchRciById(id));
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while fetching rci {id}");

                throw;
            }
        }

        /// <summary>
        /// Add a fine record to the database
        /// </summary>
        public int AddFine(RciNewFineViewModel newFine)
        {
            try
            {
                return this.Dal.CreateNewFine(newFine.FineAmount, newFine.FineOwner, newFine.FineReason, newFine.RciId, newFine.RoomComponentTypeId);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to add fine. FineReason={newFine.FineReason}, Amount={newFine.FineAmount}, GordonId={newFine.FineOwner}, RciId={newFine.RciId}, RoomComponentTypeId={newFine.RoomComponentTypeId}");

                throw;
            }
        }

        /// <summary>
        /// Delete a fine from the database
        /// </summary>
        public void RemoveFine(int fineID)
        {
            try
            {
                this.Dal.DeleteFine(fineID);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to remove fine {fineID}");

                throw;
            }
        }

        /// <summary>
        /// Let the indicated resident sign the rci. This signature is represented by adding a record to the
        /// CommonAreaRciSignature table.
        /// </summary>
        public void CheckoutCommonAreaMemberSignRci(int rciID, string gordonID)
        {
            try
            {
                this.Dal.CreateNewCommonAreaRciSignature(gordonID, rciID, DateTime.Now, Constants.CHECKOUT);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to create a new common area member signature for checkout. RciId={rciID} GordonId={gordonID}");

                throw;
            }
        }

        /// <summary>
        /// Sign the resident portion of the rci during the checkout process
        /// </summary>
        public void CheckoutResidentSignRci(int rciID)
        {
            try
            {
                this.Dal.SetRciCheckoutDateColumns(new List<int> { rciID }, DateTime.Today, null, null);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to sign the checkout for rciId={rciID}");

                throw;
            }
        }

        /// <summary>
        /// Sign the RA portion of the rci during the checkout process
        /// </summary>
        public void CheckoutRASignRci(int rciID, string raGordonID)
        {
            try
            {
                this.Dal.SetRciCheckoutDateColumns(new List<int> { rciID }, null, DateTime.Today, null);
                this.Dal.SetRciCheckoutGordonIdColumns(new List<int> { rciID }, raGordonID, null);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected error when RA user {raGordonID} tried to sign the checkout for rciId={rciID}");

                throw;
            }
        }

        /// <summary>
        /// Sign the RD portion of the rci during the checkout process and make the rci non-current.
        /// </summary>
        public void CheckoutRDSignRci(int rciID, string rdGordonID)
        {
            try
            {  
                this.Dal.SetRciCheckoutDateColumns(new List<int> { rciID }, null, null, DateTime.Today);

                this.Dal.SetRciCheckoutGordonIdColumns(new List<int> { rciID }, null, rdGordonID);
            }
            catch(Exception e)
            {
                logger.Error(e, $"Unexpected error when RD user {rdGordonID} tried to sign the checkout for rciId={rciID}");

                throw;
            }
        }

        /// <summary>
        /// Send the fine email(s) associated with the corresponding rci
        /// </summary>
        /// <param name="rciID"></param>
        public void SendFineEmail(int rciID, string emailAddress, string password)
        {
            logger.Debug($"User {emailAddress} is about to send fine emails for rciId={rciID}");

            var rci = new FullRciViewModel(this.Dal.FetchRciById(rciID));

            var fineEmailDictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var component in rci.RoomComponents)
            {
                foreach (var fine in component.Fines.Where(x => x.Amount > 0))// Work requests get input as $0 charges. We don't want to confuse students by emailng them $0 charges.
                {
                    if (fineEmailDictionary.ContainsKey(fine.GordonId))
                    {
                        fineEmailDictionary[fine.GordonId]["body"] += "<p>" + component.RoomComponentName + " - " + fine.Reason + ": $" + fine.Amount + "</p>";
                        var total = decimal.Parse(fineEmailDictionary[fine.GordonId]["total"]);
                        total = total + fine.Amount;
                        fineEmailDictionary[fine.GordonId]["total"] = total.ToString();
                    }
                    else
                    {
                        var newFineEmailContents = new Dictionary<string, string>
                        {
                            {"body",  "<p>" + component.RoomComponentName + " - " + fine.Reason + ": $" + fine.Amount + "</p>" },
                            {"total", fine.Amount.ToString() }
                        };
                        fineEmailDictionary.Add(fine.GordonId, newFineEmailContents);
                    }
                }
            }

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

                foreach (KeyValuePair<string, Dictionary<string, string>> entry in fineEmailDictionary)
                {
                    var message = new MailMessage();
                    var recepientAccount = this.Dal.FetchAccountByGordonId(entry.Key);
                    var to = recepientAccount.Email;
                    var from = emailAddress;
                    var today = DateTime.Now.ToLongDateString();
                    var recepientName = recepientAccount.FirstName;
                    message.To.Add(new MailAddress(to));
                    message.From = new MailAddress(from);
                    message.Subject = "Checkout Fines - " + rci.BuildingCode + " " + rci.RoomNumber;
                    message.Body = string.Format(Properties.Resources.FINE_EMAIL,
                        today,
                        recepientName,
                        entry.Value["body"],
                        "$" + entry.Value["total"]);
                    message.IsBodyHtml = true;

                    smtp.Send(message);
                }
            }
        }

        public void WorkRequestDamages(List<string> workRequests, string username, string password, string gordonId, int rciID, string phoneNumber)
        {
            try
            {
                logger.Debug($"Sending work requests for rci={rciID}.");

                if (workRequests == null || workRequests.Count <= 0)
                {
                    // No work requests
                    logger.Debug($"No work requests entered for rci={rciID}");

                    return;
                }

                // Go.gordon.edu can be accessed using Basic authentication. I found this out via trial and error xD
                string authenticationInfo = username + ":" + password;

                authenticationInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authenticationInfo));

                var acct = this.Dal.FetchAccountByGordonId(gordonId);

                var rci = this.Dal.FetchRciById(rciID);

                var buildingName = this.Dal.FetchBuildingCodeToBuildingNameMap()[rci.BuildingCode];

                // Set up the Http client
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constants.GO_GORDON_URL);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationInfo);
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    client.DefaultRequestHeaders.ConnectionClose = false;

                    foreach (var request in workRequests)
                    {
                        PostWorkRequest(client, request, acct.AdUsername, buildingName, rci.RoomNumber, phoneNumber, acct.FirstName, acct.LastName, acct.GordonId);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while posting a work request for rciId={rciID}. User={username}, PhoneNumber={phoneNumber}, GordonId={gordonId}. WorkRequests={string.Join(";", workRequests)}");

                throw;
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
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("submitted_by", ADUsername),
                new KeyValuePair<string, string>("ad_username", ADUsername),
                new KeyValuePair<string, string>("building", fullBuildingName),
                new KeyValuePair<string, string>("location", roomNumber),
                new KeyValuePair<string, string>("strdescription", workRequest),
                new KeyValuePair<string, string>("phone", phoneNumber),
                new KeyValuePair<string, string>("fname", firstname),
                new KeyValuePair<string, string>("lname", lastname),
                new KeyValuePair<string, string>("gordon_id", gordonID),
                new KeyValuePair<string, string>("m_date_needed", "1"),
                new KeyValuePair<string, string>("d_date_needed", "1"),
                new KeyValuePair<string, string>("y_date_needed", "1900"),
                new KeyValuePair<string, string>("hr_date_needed", "00"),
                new KeyValuePair<string, string>("mm_date_needed", "00"),
                new KeyValuePair<string, string>("tt_date_needed", "AM"),
                new KeyValuePair<string, string>("m_date_to", "1"),
                new KeyValuePair<string, string>("d_date_to", "1"),
                new KeyValuePair<string, string>("y_date_to", "1900"),
                new KeyValuePair<string, string>("hr_date_to", "00"),
                new KeyValuePair<string, string>("mm_date_to", "00"),
                new KeyValuePair<string, string>("tt_date_to", "AM")
            };

            var logMessage = new StringBuilder();

            logMessage.AppendLine($"The following form values are being sent to the go.gordon site: ");

            foreach (var kvp in data)
            {
                logMessage.AppendLine($"{kvp.Key}={kvp.Value}");
            }

            logger.Info(logMessage.ToString());

            var content = new FormUrlEncodedContent(data);

            var response = client.PostAsync(Constants.WORK_REQUEST_ENDPOINT, content).Result;

            var statusCode = response.StatusCode;

            return statusCode;
        }
    }
}