using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System.Web.Mvc;
using Phoenix.DapperDal;
using System.Text;
using Phoenix.DapperDal.Types;
using Phoenix.Exceptions;

namespace Phoenix.Services
{
    public class DashboardService : IDashboardService
    {
        private IDatabaseDal Dal { get; set; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DashboardService(IDatabaseDal dal)
        {
            this.Dal = dal;
        }

        public IEnumerable<HomeRciViewModel> GetCurrentRcisForResident(string gordonId)
        {
            try
            {
                if (gordonId == null)
                {
                    throw new ArgumentNullException(gordonId);
                }

                logger.Debug($"Fetching Rcis for resident {gordonId}");

                var currentRcis = this.Dal.FetchRcisByGordonId(gordonId)
                    .Where(x => x.IsCurrent)
                    .Select(x => new HomeRciViewModel(x));

                logger.Debug($"Got {currentRcis.Count()} back for resident {gordonId}");

                return currentRcis;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception happened while trying to get rcis for resident {gordonId}.");

                throw;
            }
        }

        public IEnumerable<HomeRciViewModel> GetCurrentRcisForBuilding(List<string> buildingCodes)
        {
            try
            {
                logger.Debug($"Fetching rcis for buildings : {string.Join(",", buildingCodes)}...");

                var currentRcis = this.Dal
                    .FetchRcisByBuilding(buildingCodes)
                    .Where(x => x.IsCurrent)
                    .Select(x => new HomeRciViewModel(x))
                    .OrderBy(m => m.RoomNumber);

                logger.Debug($"Got {currentRcis.Count()} back for buildings : {string.Join(",", buildingCodes)}...");

                return currentRcis;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception happened while trying to fetch rcis for buildings: {string.Join(",", buildingCodes)}");

                throw;
            }
        }

        public IEnumerable<HomeRciViewModel> GetCurrentCommonAreaRcisForRoom(string currentRoom, string building)
        {
            try
            {
                var apartmentNumber = currentRoom.TrimEnd(Constants.ROOM_NUMBER_SUFFIXES);

                logger.Debug($"Fetching Common Area Rcis for room {building} {currentRoom}. Apartment name was determined to be {apartmentNumber}");

                var currentCommonAreaRcis = this.Dal.FetchRcisForRoom(building, apartmentNumber)
                    .Where(x => x.IsCurrent) // Current
                    .Where(x => x.GordonId == null) // Common Area
                    .Select(x => new HomeRciViewModel(x));

                logger.Debug($"Got {currentCommonAreaRcis.Count()} rcis back for room {building} {currentRoom}");

                return currentCommonAreaRcis;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while fetching rcis for room {building} {currentRoom}");

                throw;
            }
        }

         /// <summary>
         /// Generate a csv file of fines for rci's from current, according to buildings of RD
         /// </summary>
         /// <param name="buildingCodes">The building for which to generate a fines spreadsheet</param>
        public string GenerateFinesSpreadsheet(List<string> buildingCodes)
        {
            try
            {
                var currentSession = GetCurrentSession();

                logger.Debug($"Generating fines spreadsheet. The current session is {currentSession}");

                var finesCsv = new StringBuilder();

                finesCsv.AppendLine(@"""Room Number"",""Building Code"",""Name"",""ID"",""Detailed Reason"",""Charge Amount"",""Behavioral Fine""");

                var fines = this.Dal.FetchFinesByBuilding(buildingCodes)
                    .Where(x => x.IsCurrent)
                    .Where(x => x.FineAmount > 0); // Don't include $0 fines in the query. These will be present when an RD wants to work request something, but not
                                                   // charge the resident for it e.g. Window blinds need to be replaced.

                logger.Debug($"Got {fines.Count()} fines for buildings {string.Join(",", buildingCodes)}");

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
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while generating fine summary for buildings {string.Join(",", buildingCodes)}");

                throw;
            }
        } 

        public string GetCurrentSession()
        {
            try
            {
                var currentSession = this.Dal.FetchCurrentSession();

                logger.Debug($"Call to GetCurrentSession - {currentSession}");

                return currentSession;
            }
            catch (CurrentSessionNotFoundException e)
            {
                // We could just return the most recent session but that would mess up things that require sessions to match? I don't currently have a good example though.
                // So for now, just throw.

                logger.Error(e, $"Could not determine the current session! Date is {DateTime.Now}");

                throw;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to get current session");

                throw;
            }
        } 

        /// <summary>
        /// Make sure the rci table is up to date with the room assign table for the specified kingdom
        /// </summary>
        public void SyncRoomRcisFor(List<string> kingdom)
        {
            try
            {
                logger.Info($"Rci Generation for the following buildings is about to begin: {string.Join(",", kingdom)}...");

                var result = Enumerable.Empty<RoomAssignment>();

                var currentSession = this.GetCurrentSession();

                // Find the room assign records that are missing rcis
                foreach (var building in kingdom)
                {
                    var query = this.Dal.FetchRoomAssignmentsThatDoNotHaveRcis(building, currentSession);

                    result = result.Concat(query);
                }

                logger.Info($"For buildings {string.Join(",", kingdom)}, there were {result.Count()} room assignments without matching Rcis...");

                // Create the rcis
                foreach (var roomAssignment in result)
                {
                    if (roomAssignment.GordonId == null || roomAssignment.BuildingCode == null || roomAssignment.RoomNumber == null)
                    {
                        logger.Warn($"Encountered room assignment with null values for either GordonId, Building Code or RoomNumber. GordonId{roomAssignment.GordonId}, BuildingCode={roomAssignment.BuildingCode}, RoomNumber={roomAssignment.RoomNumber}..");

                        continue;
                    }

                    var newRciId = this.Dal.CreateNewDormRci(roomAssignment.GordonId, roomAssignment.BuildingCode, roomAssignment.RoomNumber, roomAssignment.SessionCode);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception during Rci Generation process...");

                throw;
            }
        }

        /// <summary>
        /// Make sure the rci table is up to date with the room assign table for the specified room
        /// </summary>
        public void SyncRoomRcisFor(string buildingCode, string roomNumber, string idNumber)
        {
            try
            {
                logger.Debug($"Rci Generation process for room {buildingCode} {roomNumber} is starting. GordonId is {idNumber}...");

                // Find all rcis for the person
                var myRcis = this.Dal
                    .FetchRcisByGordonId(idNumber)
                    .Where(x => x.BuildingCode == buildingCode)
                    .Where(x => x.RoomNumber == roomNumber);

                RoomAssignment mostRecentRoomAssign;

                try
                {
                    mostRecentRoomAssign = this.Dal.FetchLatestRoomAssignmentForId(idNumber);
                }
                catch(RoomAssignNotFoundException)
                {
                    // This person has no previous room assignments.
                    mostRecentRoomAssign = null;
                }

                logger.Debug($"Person {idNumber} has {myRcis.Count()} total Rcis (Including archived ones)");

                // Get most recent rci.
                var mostRecentRci = myRcis.OrderByDescending(m => m.CreationDate).FirstOrDefault();

                logger.Debug($"Most recent Rci for Person {idNumber} is  {mostRecentRci?.RciId}. Their most recent room assignment date is {mostRecentRoomAssign?.AssignmentDate}");

                var createNewRci = false;

                // There are room assign records for this person but no rcis.
                if (mostRecentRci == null && mostRecentRoomAssign != null)
                {
                    createNewRci = true;
                }
                // This will happen if there is no room assign record for the person
                else if (mostRecentRci != null && mostRecentRoomAssign == null)
                {
                    createNewRci = false;
                }
                // For people who have no room assignments and no rcis.
                else if (mostRecentRci == null && mostRecentRoomAssign == null)
                {
                    createNewRci = false;
                }
                // Both values are non-null.
                else
                {
                    // Compare Creation date of rci and assign date of room assign record
                    createNewRci = mostRecentRci.CreationDate < mostRecentRoomAssign.AssignmentDate;
                }

                logger.Debug($"The boolean variable createNewRci for person {idNumber} is {createNewRci}...");

                if (createNewRci)
                {
                    var roomAssignment = this.Dal.FetchLatestRoomAssignmentForId(idNumber);

                    this.Dal.CreateNewDormRci(idNumber, buildingCode, roomNumber, roomAssignment.SessionCode);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while going through the rci generation process for room {buildingCode} {roomNumber} for person {idNumber}");

                throw;
            }
        }

        /// <summary>
        /// Create rci records that correspond to common areas in the Room table for the specified room.
        /// End result: If there isn't an active common area rci associated with the given room, always create one.
        /// </summary>
        public void SyncCommonAreaRcisFor(string buildingCode, string roomNumber)
        {
            try
            {
                logger.Debug($"Starting Rci Generation process for the common area {buildingCode} {roomNumber}...");

                var apartmentNumber = roomNumber.TrimEnd(Constants.ROOM_NUMBER_SUFFIXES);

                logger.Debug($"{roomNumber} -> {apartmentNumber}");

                // Room types that would flag apartment-style room records
                var apartmentFlags = new List<string>() { "AP", "LV" };

                // Do I already have an active common area rci for the apartment I am in?
                var activeCommonAreaRcis = this.Dal
                    .FetchRcisForRoom(buildingCode, apartmentNumber)
                    .Where(x => x.IsCurrent)
                    .Where(x => x.GordonId == null); // This indicates a common area rci

                logger.Debug($"Got {activeCommonAreaRcis.Count()} active (current) rcis for room {buildingCode} {apartmentNumber}...");

                var commonAreaRciExists = activeCommonAreaRcis.Any();

                // Does the room I live in have a common area?
                Room thisRoom;

                try
                {
                    thisRoom = this.Dal.FetchRoom(buildingCode, apartmentNumber);
                }
                catch (RoomNotFoundException e)
                {
                    logger.Error($"Room {buildingCode} {apartmentNumber} was not found! Common Area won't be created for it...", e);

                    // Well with this exception, clearly this room cannot be found in the Room table.
                    // I choose to take a simple, safe route: if i can't find the room, i ain't creating a common area for it :)

                    return;
                }

                var commonAreaExists = apartmentFlags.Contains(thisRoom.RoomType);

                logger.Debug($"Found room {buildingCode} {apartmentNumber}. It was of type {thisRoom.RoomType}");

                // There is a common area rci to create if a common area exists for that room AND 
                // the person has no active common area rcis for that room
                var createCommonAreaRci = commonAreaExists && !commonAreaRciExists;

                if (createCommonAreaRci)
                {
                    logger.Debug($"Common Area Rci is being created for room {buildingCode} {apartmentNumber}...");

                    var commmonAreaRciId = this.Dal.CreateNewCommonAreaRci(thisRoom.BuildingCode, thisRoom.RoomNumber, this.GetCurrentSession());
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception when performing Rci Generation process for Common Areas. Building={buildingCode}, RoomNumber={roomNumber}");

                throw;
            }
        }

        /// <summary>
        /// Set the IsCurrent column for a bunch of rcis to false.
        /// </summary>
        public void ArchiveRcis(List<int> rciIds)
        {
            try
            {
                logger.Debug($"Archiving the following Ids... {string.Join(",", rciIds)}");

                this.Dal.SetRciIsCurrentColumn(rciIds, false);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while archiving rcis {string.Join(",", rciIds)}");

                throw;
            }
        }

        /// <summary>
        /// Swap the damages in these two rcis.
        /// </summary>
        public void SwapRciDamages(int rciId1, int rciId2)
        {
            try
            {
                logger.Debug($"Swapping rcis {rciId1} {rciId2}");

                var rci1 = this.Dal.FetchRciById(rciId1);

                var rci2 = this.Dal.FetchRciById(rciId2);

                var firstRciDamages = rci1.Damages.Select(x => x.DamageId);

                var secondRciDamages = rci2.Damages.Select(x => x.DamageId);

                this.Dal.UpdateDamage(firstRciDamages, null, null, rci2.RciId, null);

                this.Dal.UpdateDamage(secondRciDamages, null, null, rci1.RciId, null);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to swap rcis {rciId2} and {rciId1}");

                throw;
            }
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