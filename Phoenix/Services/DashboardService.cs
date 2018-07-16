using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System.Web.Mvc;
using Phoenix.DapperDal;
using System.Text;
using Phoenix.DapperDal.Types;

namespace Phoenix.Services
{
    public class DashboardService : IDashboardService
    {
        private IDal Dal { get; set; }

        private ILoggerService Logger { get; set; }

        public DashboardService(IDal dal, ILoggerService logger)
        {
            this.Dal = dal;

            this.Logger = logger;
        }

        public IEnumerable<HomeRciViewModel> GetCurrentRcisForResident(string gordonId)
        {
            if (gordonId == null)
            {
                return null;
            }

            var currentRcis = this.Dal.FetchRcisByGordonId(gordonId)
                .Where(x => x.IsCurrent)
                .Select(x => new HomeRciViewModel(x));

            return currentRcis;
        }

        public IEnumerable<HomeRciViewModel> GetCurrentRcisForBuilding(List<string> buildingCodes)
        {
            var currentRcis = this.Dal
                .FetchRcisByBuilding(buildingCodes)
                .Where(x => x.IsCurrent)
                .Select(x => new HomeRciViewModel(x))
                .OrderBy(m => m.RoomNumber);

            return currentRcis;
        }

        public IEnumerable<HomeRciViewModel> GetCurrentCommonAreaRcisForRoom(string currentRoom, string building)
        {
            var apartmentNumber = currentRoom.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });

            var currentCommonAreaRcis = this.Dal.FetchRcisForRoom(building, apartmentNumber)
                .Where(x => x.IsCurrent) // Current
                .Where(x => x.GordonId == null) // Common Area
                .Select(x => new HomeRciViewModel(x));

            return currentCommonAreaRcis;
        }

         /// <summary>
         /// Generate a csv file of fines for rci's from current, according to buildings of RD
         /// </summary>
         /// <param name="buildingCodes">The building for which to generate a fines spreadsheet</param>
        public string GenerateFinesSpreadsheet(List<string> buildingCodes)
        {
            var currentSession = GetCurrentSession();
            var finesCsv = new StringBuilder();
            finesCsv.AppendLine(@"""Room Number"",""Building Code"",""Name"",""ID"",""Detailed Reason"",""Charge Amount"",""Behavioral Fine""");

            var fines = this.Dal.FetchFinesByBuilding(buildingCodes)
                .Where(x => x.IsCurrent)
                .Where(x => x.FineAmount > 0); // Don't include $0 fines in the query. These will be present when an RD wants to work request something, but not
                                               // charge the resident for it e.g. Window blinds need to be replaced.

            foreach (var fine in fines)
            {
                // All the fields are quoted because some fields will contain the comma separater in them, which will throw things off if the quotes are not present.
                finesCsv.Append($"\"{fine.RoomNumber}\",");
                finesCsv.Append($"\"{fine.BuildingCode}\",");
                finesCsv.Append($"\"{fine.FirstName} {fine.LastName}\",");
                finesCsv.Append($"\"{fine.GordonId}\",");
                finesCsv.Append($"\"{fine.RoomComponentName}: {fine.Reason}\",");
                finesCsv.Append($"\"{fine.FineAmount}\",");
                finesCsv.Append(fine.RoomArea.Equals(Constants.FINE, StringComparison.OrdinalIgnoreCase) ? $"\"YES\"" : $"\"NO\"");
                finesCsv.AppendLine();
            }

            return finesCsv.ToString();
        } 

        public string GetCurrentSession()
        {
            var today = DateTime.Now;

            var allSessions = this.Dal.FetchSessions();

            var sessions = allSessions
                .Where(m => m.SessionStartDate.HasValue && m.SessionEndDate.HasValue)
                .Where(x => today.CompareTo(x.SessionStartDate.Value) >= 0
                            &&
                            today.CompareTo(x.SessionEndDate.Value) <= 0); // We are assuming sessions don't overlap

            var currentSession = sessions.FirstOrDefault();

            // If we are within a session.
            if(currentSession != null)
            {
                return currentSession.SessionCode.Trim();
            }
            // If the table doesn't have a session for the date we are within
            else
            {
                return allSessions.OrderByDescending(m => m.SessionStartDate).First().SessionCode.Trim();
            }
        } 

        /// <summary>
        /// Make sure the rci table is up to date with the room assign table for the specified kingdom
        /// </summary>
        public void SyncRoomRcisFor(List<string> kingdom)
        {
            var result = Enumerable.Empty<RoomAssignment>();

            var currentSession = this.GetCurrentSession();

            // Find the room assign records that are missing rcis
            foreach (var building in kingdom)
            {
                var query = this.Dal.FetchRoomAssignmentsThatDoNotHaveRcis(building, currentSession);

                result = result.Concat(query);
            }

            // Create the rcis
            foreach (var roomAssignment in result)
            {
                if (roomAssignment.GordonId == null || roomAssignment.BuildingCode == null || roomAssignment.RoomNumber == null)
                {
                    continue;
                }

                var newRciId = this.Dal.CreateNewDormRci(roomAssignment.GordonId, roomAssignment.BuildingCode, roomAssignment.RoomNumber, roomAssignment.SessionCode);

                this.Logger.Info($"New Rci Created by Rci Generation in DashboardService.SyncRoomRcisFor. RciId={newRciId}");
            }
        }

        /// <summary>
        /// Make sure the rci table is up to date with the room assign table for the specified room
        /// </summary>
        public void SyncRoomRcisFor(string buildingCode, string roomNumber, string idNumber, DateTime? roomAssignDate)
        {
            // Find all rcis for the person
            var myRcis = this.Dal
                .FetchRcisByGordonId(idNumber)
                .Where(x => x.BuildingCode == buildingCode)
                .Where(x => x.RoomNumber == roomNumber);

            // Get most recent rci.
            var mostRecentRci = myRcis.OrderByDescending(m => m.CreationDate).FirstOrDefault();


            var createNewRci = false;

            // There are room assign records for this person but no rcis.
            if(mostRecentRci == null && roomAssignDate != null) 
            {
                createNewRci = true;
            }
            // This will happen if there is no room assign record for the person
            else if(mostRecentRci != null && roomAssignDate == null)
            {
                createNewRci = false;
            }
            // Both values are non-null.
            else
            {
                // Compare Creation date of rci and assign date of room assign record
                createNewRci = mostRecentRci.CreationDate < roomAssignDate;
            }
            
            if(createNewRci)
            {
                var roomAssignment = this.Dal.FetchLatestRoomAssignmentForId(idNumber);

                this.Dal.CreateNewDormRci(idNumber, buildingCode, roomNumber, roomAssignment.SessionCode);
            }
        }

        /// <summary>
        /// Create rci records that correspond to common areas in the Room table for the specified room.
        /// End result: If there isn't an active common area rci associated with the given room, always create one.
        /// </summary>
        public void SyncCommonAreaRcisFor(string buildingCode, string roomNumber)
        {
            var apartmentNumber = roomNumber.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });

            // Room types that would flag apartment-style room records
            var apartmentFlags = new List<string>() { "AP", "LV" };

            // Do I already have an active common area rci for the apartment I am in?
            var activeCommonAreaRcis = this.Dal
                .FetchRcisForRoom(buildingCode, apartmentNumber)
                .Where(x => x.IsCurrent)
                .Where(x => x.GordonId == null); // This indicates a common area rci

            var commonAreaRciExists = activeCommonAreaRcis.Any();

            // Does the room I live in have a common area?
            var thisRoom = this.Dal.FetchRoom(buildingCode, apartmentNumber);
            var commonAreaExists = apartmentFlags.Contains(thisRoom.RoomType);

            // There is a common area rci to create if a common area exists for that room AND 
            // the person has no active common area rcis for that room
            var createCommonAreaRci = commonAreaExists && !commonAreaRciExists;

            if(createCommonAreaRci)
            {
                var commmonAreaRciId = this.Dal.CreateNewCommonAreaRci(thisRoom.BuildingCode, thisRoom.RoomNumber, this.GetCurrentSession());

                this.Logger.Info($"New Common Area Rci Created by Rci Generation in DashboardService.SyncCommonAreaRcisFor. RciId={commmonAreaRciId}");
            }
        }

        /// <summary>
        /// Set the IsCurrent column for a bunch of rcis to false.
        /// </summary>
        public void ArchiveRcis(List<int> rciIds)
        {
            this.Dal.SetRciIsCurrentColumn(rciIds, false);
        }

        /// <summary>
        /// Get a string that represents the state of the rci.
        /// </summary>
        public string GetRciState(int rciID)
        {
            var rci = this.Dal.FetchRciById(rciID);

            if(rci.ResidentCheckinDate == null)
            {
                return Constants.RCI_UNSIGNED;
            }
            else if(rci.RaCheckinDate == null)
            {
                return Constants.RCI_SIGNGED_BY_RES_CHECKIN;
            }
            else if(rci.RdCheckinDate == null)
            {
                return Constants.RCI_SIGNGED_BY_RA_CHECKIN;
            }
            else if(rci.ResidentCheckoutDate == null)
            {
                return Constants.RCI_SIGNGED_BY_RD_CHECKIN;
            }
            else if(rci.RaCheckoutDate == null)
            {
                return Constants.RCI_SIGNGED_BY_RES_CHECKOUT;
            }
            else if(rci.RdCheckoutDate == null)
            {
                return Constants.RCI_SIGNGED_BY_RA_CHECKOUT;
            }
            else // rci.RdCheckoutDate != null
            {
                return Constants.RCI_COMPLETE;
            }
        }

        /// <summary>
        /// Create a route dictionary that will tell us where to go given the rci state and the role of the user.
        /// </summary>
        public Dictionary<string, Dictionary<string, ActionResult>> GetRciRouteDictionary(int rciID)
        {
            var rciRouteDictionary = new Dictionary<string, Dictionary<string, ActionResult>>();

            rciRouteDictionary.Add(Constants.RCI_UNSIGNED, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                 {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))}
            });

            rciRouteDictionary.Add(Constants.RCI_SIGNGED_BY_RES_CHECKIN, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                 {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))}
            });

            rciRouteDictionary.Add(Constants.RCI_SIGNGED_BY_RA_CHECKIN, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciInput", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciInput", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))},
                 {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciInput", id = rciID }))}
            });

            rciRouteDictionary.Add(Constants.RCI_SIGNGED_BY_RD_CHECKIN, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciInput", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))},
                 {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))}
            });

            rciRouteDictionary.Add(Constants.RCI_SIGNGED_BY_RES_CHECKOUT, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciCheckout", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))},
                {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))}
            });

            rciRouteDictionary.Add(Constants.RCI_SIGNGED_BY_RA_CHECKOUT, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciCheckout", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciCheckout", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))},
                {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))}
            });

            rciRouteDictionary.Add(Constants.RCI_COMPLETE, new Dictionary<string, ActionResult>
            {
                {Constants.RESIDENT,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciCheckout", id = rciID }))},
                {Constants.RA,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciCheckout", id = rciID }))},
                {Constants.RD,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "RciReview", controller="RciCheckout", id = rciID }))},
                {Constants.ADMIN,
                    new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", controller="RciCheckout", id = rciID }))}
            });


            return rciRouteDictionary;
        }
    }
}