using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Services
{
    public class DashboardService
{
        private RCIContext db;
        private XDocument document;
        public DashboardService()
        {
            db = new Models.RCIContext();
            document = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        public DashboardService(bool testflag)
        {
            db = new Models.RCIContext();
        }

        /// <summary>
        /// Get a set of RCI's, depending on certain parameters, such as building, year, etc.
        /// This method is used in admin Find RCIs tool
        /// If no params are specified, returns all
        /// </summary>
        /// <param name="building">a building specified to get rci's for</param>
        /// <param name="year"> a session specificed to get rci's for</param>
        /// <returns>A collection of RCI View Models</returns>
        public IEnumerable<HomeRciViewModel> GetRcis(string building = null, string year = null)
        {
            if (building != null)
            {
                return GetRcisForBuilding(new List<string> { building });
            }
            else 
            {
                // Return all RCI's
                var RCIs = from personalRCI in db.Rci
                           join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                           select new HomeRciViewModel
                           {
                               RciID = personalRCI.RciID,
                               BuildingCode = personalRCI.BuildingCode.Trim(),
                               RoomNumber = personalRCI.RoomNumber.Trim(),
                               FirstName = account.firstname,
                               LastName = account.lastname,
                               RciStage = personalRCI.CheckinSigRD == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE,
                               CheckinSigRes = personalRCI.CheckinSigRes,
                               CheckinSigRA = personalRCI.CheckinSigRA,
                               CheckinSigRD = personalRCI.CheckinSigRD,
                               CheckoutSigRes = personalRCI.CheckoutSigRes,
                               CheckoutSigRA = personalRCI.CheckoutSigRA,
                               CheckoutSigRD = personalRCI.CheckoutSigRD
                           };
                return RCIs;
            }
        }

        ///<summary>
        /// Get the RCI for an individual resident
        ///</summary>
        ///<param  name="id">resident's Gordon id </params> 
        ///<returns>A collection of RCI View Models (which should contain only 1)</returns>
        public IEnumerable<HomeRciViewModel> GetRcisForResident(string id)
        {
            if(id == null)
            {
                return null;
            }
            var RCIs =
                from personalRCI in db.Rci
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                where account.ID_NUM == id && personalRCI.IsCurrent == true
                select new HomeRciViewModel
                {
                    RciID = personalRCI.RciID,
                    BuildingCode = personalRCI.BuildingCode.Trim(),
                    RoomNumber = personalRCI.RoomNumber.Trim(),
                    FirstName = account.firstname,
                    LastName = account.lastname,
                    RciStage = personalRCI.CheckinSigRD == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE,
                    CheckinSigRes = personalRCI.CheckinSigRes,
                    CheckinSigRA = personalRCI.CheckinSigRA,
                    CheckinSigRD = personalRCI.CheckinSigRD,
                    CheckoutSigRes = personalRCI.CheckoutSigRes,
                    CheckoutSigRA = personalRCI.CheckoutSigRA,
                    CheckoutSigRD = personalRCI.CheckoutSigRD
                };
            return RCIs;
        }

        ///<summary>
        /// Display all RCI's for the corresponding building 
        ///</summary>
        ///<param name="buildingCode">code(s) for the building(s) of the RA or RD's sphere of authority</param>
        public IEnumerable<HomeRciViewModel> GetRcisForBuilding(List<string> buildingCode)
        {
            var buildingRCIs =
                from personalRCI in db.Rci
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where buildingCode.Contains(personalRCI.BuildingCode) && personalRCI.IsCurrent == true
                select new HomeRciViewModel
                {
                    RciID = personalRCI.RciID,
                    BuildingCode = personalRCI.BuildingCode.Trim(),
                    RoomNumber = personalRCI.RoomNumber.Trim(),
                    FirstName = account.firstname == null ? "Common Area" : account.firstname,
                    LastName = account.lastname == null ? "RCI" : account.lastname,
                    RciStage = personalRCI.CheckinSigRD == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE,
                    CheckinSigRes = personalRCI.CheckinSigRes,
                    CheckinSigRA = personalRCI.CheckinSigRA,
                    CheckinSigRD = personalRCI.CheckinSigRD,
                    CheckoutSigRes = personalRCI.CheckoutSigRes,
                    CheckoutSigRA = personalRCI.CheckoutSigRA,
                    CheckoutSigRD = personalRCI.CheckoutSigRD
                };
            return buildingRCIs.OrderBy(m => m.RoomNumber);
        }


        ///<summary>
        /// Create RCI Components that are associated with a single RCI, according to room type
        ///</summary>
        ///<param name="document">Xml document containing room components</param>
        ///<param name="rciId">the id of the RCI to associate with </param>
        ///<param name="roomType">string to indicate type of room, either "common area" or "dorm room" currently</param>
        ///<param name="buildingCode">building code for the rci we are about to create</param>
        public List<RciComponent> CreateRciComponents(XDocument document, int rciId, string roomType, string buildingCode)
        {
            List<RciComponent> created = new List<RciComponent>();

            XElement rciTypes = document.Root;
            IEnumerable<XElement> componentElements =
                from rci in rciTypes.Elements("rci")
                where ((string)rci.Attribute("roomType")).Equals(roomType) && (string) rci.Attribute("buildingCode") == buildingCode
                from component in rci.Element("components").Elements("component")
                select component;

            foreach (var componentElement in componentElements)
            {
                var newComponent = new RciComponent();
                newComponent.RciComponentName = (string)componentElement.Attribute("name");
                newComponent.RciComponentDescription = (string)componentElement.Attribute("description");
                newComponent.RciID = rciId;

                var costString = "";
                var costElements = componentElement.Elements("cost");
                foreach(var costElement in costElements)
                {
                    var costReason = costElement.Attribute("name").Value;
                    var costAmount = costElement.Attribute("approxCost").Value;
                    costString += costReason + "," + costAmount + "|";
                }

                newComponent.SuggestedCosts = costString;
                created.Add(newComponent);
            }
            return created;
        }


        ///<summary>
        /// Get the current common area RCIs for an apartment
        ///</summary>
        ///<param name="apartmentNumber">the apartment's number</param>
        ///<param name="building">the building where the apartment is located</param>
        ///<returns>the common area, if any, that was found in the db</returns>
        public IEnumerable<HomeRciViewModel> GetCommonAreaRci(string currentRoom, string building)
        {
            var apartmentNumber = currentRoom.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });

            var commonAreaRCIs =
                from tempCommonAreaRCI in db.Rci
                where tempCommonAreaRCI.RoomNumber == apartmentNumber && tempCommonAreaRCI.BuildingCode == building
                && tempCommonAreaRCI.GordonID == null && tempCommonAreaRCI.IsCurrent == true
                select new HomeRciViewModel
                {
                    RciID = tempCommonAreaRCI.RciID,
                    BuildingCode = tempCommonAreaRCI.BuildingCode,
                    RoomNumber = tempCommonAreaRCI.RoomNumber,
                    FirstName = "Common Area",
                    LastName = "RCI",
                    RciStage = tempCommonAreaRCI.CheckinSigRD == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE,
                    CheckinSigRes = tempCommonAreaRCI.CheckinSigRes,
                    CheckinSigRA = tempCommonAreaRCI.CheckinSigRA,
                    CheckinSigRD = tempCommonAreaRCI.CheckinSigRD,
                    CheckoutSigRes = tempCommonAreaRCI.CheckoutSigRes,
                    CheckoutSigRA = tempCommonAreaRCI.CheckoutSigRA,
                    CheckoutSigRD = tempCommonAreaRCI.CheckoutSigRD
                };
            return commonAreaRCIs;

        }

         /// <summary>
         /// Generate a csv file of fines for rci's from current, according to buildings of RD
         /// </summary>
         /// <param name="buildingCodes">The building for which to generate a fines spreadsheet</param>
        public string GenerateFinesSpreadsheet(List<string> buildingCodes)
        {
            var currentSession = GetCurrentSession();
            var csvString = "Room Number,Building Code,Name,ID,Detailed Reason,Charge Amount,Behavioral Fine\n";

            var fineQueries =
                from rci in db.Rci
                join component in db.RciComponent on rci.RciID equals component.RciID
                join fine in db.Fine on component.RciComponentID equals fine.RciComponentID
                join account in db.Account on fine.GordonID equals account.ID_NUM
                where buildingCodes.Contains(rci.BuildingCode) && rci.IsCurrent.Value == true
                && fine.FineAmount > 0 
                // Don't include $0 fines in the query. These will be present when an RD wants to work request something, but not
                // charge the resident for it e.g. Window blinds need to be replaced.
                select new
                {
                    RoomNumber = rci.RoomNumber,
                    BuildingCode = rci.BuildingCode,
                    FirstName = account.firstname,
                    LastName = account.lastname,
                    Id = fine.GordonID,
                    ComponentName = component.RciComponentName,
                    DetailedReason = fine.Reason,
                    FineAmount = fine.FineAmount,
                    IsFine = component.RciComponentDescription.Equals(Constants.FINE) ? "YES" : "NO"
                };

            foreach (var fine in fineQueries)
            {
                csvString += fine.RoomNumber + ",";
                csvString += fine.BuildingCode + ",";
                csvString += fine.FirstName + " " + fine.LastName + ",";
                csvString += fine.Id + ",";
                csvString += fine.ComponentName + ": " + fine.DetailedReason + ",";
                csvString += fine.FineAmount + ",";
                csvString += fine.IsFine + "\n";
            }

            return csvString;
        } 

        ///<summary>
        /// Query the db for the current session, which will be the session with the most recent SESS_BEGN_DTE
        ///</summary>
        ///<returns>string that contains the code for the current session</returns>
         public string GetCurrentSession()
        {
            var today = DateTime.Now;
            var sessions = db.Session.Where(m => m.SESS_BEGN_DTE.HasValue && m.SESS_END_DTE.HasValue);
            sessions = sessions.Where(x => 
                            today.CompareTo(x.SESS_BEGN_DTE.Value) >= 0 
                            && 
                            today.CompareTo(x.SESS_END_DTE.Value) <= 0 ); // We are assuming sessions don't overlap
            var currentSession = sessions.FirstOrDefault();
            // If we are within a session.
            if(currentSession != null)
            {
                return currentSession.SESS_CDE.Trim();
            }
            // If the table doesn't have a session for the date we are within
            else
            {
                return db.Session.OrderByDescending(m => m.SESS_BEGN_DTE).FirstOrDefault().SESS_CDE.Trim();
            }
        } 

        /// <summary>
        /// Make sure the rci table is up to date with the room assign table for the specified kingdom
        /// </summary>
        public void SyncRoomRcisFor(List<string> kingdom)
        {
            var result = Enumerable.Empty<RoomAssign>();
            var currentSession = GetCurrentSession();

            // Find the room assign records that are missing rcis
            foreach (var building in kingdom)
            {
                // Create sql parameters that we will pass to the stored procedure
                var buildingParameter = new SqlParameter("@building", building);
                var currentSessionParameter = new SqlParameter("@currentSession", currentSession);
                
                // call the stored procedure.
                // We are using a stored procedure here because linq only supports equality joins. This operation uses a greater than join, so we execute it directly.
                var query = db.Database.SqlQuery<RoomAssign>("FindMissingRcis @building, @currentSession", buildingParameter, currentSessionParameter).AsEnumerable();
                result = result.Concat(query);

            }

            // Create the rcis
            var newRcis = new List<Rci>();

            foreach (var roomAssignment in result)
            {
                var newRci = CreateRciObject(
                    roomAssignment.BLDG_CDE.Trim(),
                    roomAssignment.ROOM_CDE.Trim(),
                    currentSession,
                    roomAssignment.ID_NUM.ToString());

                newRcis.Add(newRci);
            }

            db.Rci.AddRange(newRcis);

            db.SaveChanges();

            // Create the components
            var newComponents = new List<RciComponent>();

            foreach (var rci in newRcis)
            {
                newComponents.AddRange(CreateRciComponents(document, rci.RciID, "individual", rci.BuildingCode));
            }

            db.RciComponent.AddRange(newComponents);
            db.SaveChanges();
        }

        /// <summary>
        /// Make sure the rci table is up to date with the room assign table for the specified room
        /// </summary>
        public void SyncRoomRcisFor(string buildingCode, string roomNumber, string idNumber, DateTime? roomAssignDate)
        {
            // Find all rcis for the person
            var myRcis =
                from rci in db.Rci
                where rci.GordonID == idNumber
                && rci.BuildingCode == buildingCode
                && rci.RoomNumber == roomNumber
                select rci;
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
                var newRci = CreateRciObject(
                    buildingCode,
                    roomNumber,
                    GetCurrentSession(),
                    idNumber);

                db.Rci.Add(newRci);
                db.SaveChanges();

                // Create Components
                db.RciComponent.AddRange(CreateRciComponents(document, newRci.RciID, "individual", newRci.BuildingCode));
                db.SaveChanges();

                
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
            var activeCommonAreaRcis =
                from rci in db.Rci
                where rci.BuildingCode.Equals(buildingCode)
                && rci.RoomNumber.Equals(apartmentNumber)
                && rci.IsCurrent == true
                && rci.GordonID == null // This is what indicates a common area rci
                select rci;

            var commonAreaRciExists = activeCommonAreaRcis.Any();

            // Does the room I live in have a common area?
            var commonArea =
                from rm in db.Room
                where rm.BLDG_CDE.Equals(buildingCode)
                && rm.ROOM_CDE.Equals(apartmentNumber)
                && apartmentFlags.Contains(rm.ROOM_TYPE)
                select rm;

            var commonAreaExists = commonArea.Any();

            // There is a common area rci to create if a common area exists for that room AND 
            // the person has no active common area rcis for that room
            var createCommonAreaRci = commonAreaExists && !commonAreaRciExists;

            if(createCommonAreaRci)
            {
                var commonAreaRoom = commonArea.First();
                    
                var commonAreaRci = CreateRciObject(
                    commonAreaRoom.BLDG_CDE.Trim(),
                    commonAreaRoom.ROOM_CDE.Trim(),
                    GetCurrentSession());

                db.Rci.Add(commonAreaRci);
                db.SaveChanges();

                // Create Components
                db.RciComponent.AddRange(CreateRciComponents(document, commonAreaRci.RciID, "common", commonAreaRci.BuildingCode));
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Set the IsCurrent column for a bunch of rcis to false.
        /// </summary>
        public void ArchiveRcis(List<int> rciIds)
        {
            var rcis = db.Rci.Where(r => rciIds.Contains(r.RciID));
            foreach(var rci in rcis)
            {
                rci.IsCurrent = false;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Helper method to create and return an Rci Object. Makes no calls to the database
        /// </summary>
        public Rci CreateRciObject(string buildingCode, string roomNumber, string sessionCode, string idNumber = null)
        {
            var rci = new Rci
            {
                IsCurrent = true,
                BuildingCode = buildingCode,
                RoomNumber = roomNumber,
                SessionCode = sessionCode,
                GordonID = idNumber,
                CreationDate = DateTime.Now
            };
            return rci;
        }

        /// <summary>
        /// Get a string that represents the state of the rci.
        /// </summary>
        public string GetRciState(int rciID)
        {
            var rci = db.Rci.Find(rciID);

            if(rci.CheckinSigRes == null)
            {
                return Constants.RCI_UNSIGNED;
            }
            else if(rci.CheckinSigRA == null)
            {
                return Constants.RCI_SIGNGED_BY_RES_CHECKIN;
            }
            else if(rci.CheckinSigRD == null)
            {
                return Constants.RCI_SIGNGED_BY_RA_CHECKIN;
            }
            else if(rci.CheckoutSigRes == null)
            {
                return Constants.RCI_SIGNGED_BY_RD_CHECKIN;
            }
            else if(rci.CheckoutSigRA == null)
            {
                return Constants.RCI_SIGNGED_BY_RES_CHECKOUT;
            }
            else if(rci.CheckoutSigRD == null)
            {
                return Constants.RCI_SIGNGED_BY_RA_CHECKOUT;
            }
            else // rci.CheckoutSigRD != null
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