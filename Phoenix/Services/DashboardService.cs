using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Diagnostics;

namespace Phoenix.Services
{
    public class DashboardService
{
        private RCIContext db;
        public DashboardService()
        {
            db = new Models.RCIContext();
        }

        /*
         * Get the RCI for an individual resident
         * @params: id - resident's Gordon id
         * @return: A collection of RCI View Models (which should contain only 1)
         */ 
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
                    LastName = account.lastname
                };
            return RCIs;
        }

        /*
         * Display all RCI's for the corresponding building 
         * @params: buildingCode - code(s) for the building(s) of the RA or RD's sphere of authority
         *          gordonId - gordon ID of the RA or RD so that his or her RCI (if applicable) is not included 
         */
        public IEnumerable<HomeRciViewModel> GetRcisForBuilding(List<string> buildingCode, string gordonId)
        {
            // Not sure if this will end up with duplicates for the RA's own RCI
            var buildingRCIs =
                from personalRCI in db.Rci
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where buildingCode.Contains(personalRCI.BuildingCode) && personalRCI.IsCurrent == true
                && personalRCI.GordonID != gordonId
                select new HomeRciViewModel
                {
                    RciID = personalRCI.RciID,
                    BuildingCode = personalRCI.BuildingCode.Trim(),
                    RoomNumber = personalRCI.RoomNumber.Trim(),
                    FirstName = account.firstname == null ? "Common Area" : account.firstname,
                    LastName = account.lastname == null ? "RCI" : account.lastname
                };
            return buildingRCIs;
        }

        /*
         * Resolve inconsistencies in RD building code naming conventions in the db
         * @params: building code as it appears in CurrentRD table
         * @return: building codes that correspond to RoomAssign and RCI tables
         */ 
        public string [] CollectRDBuildingCodes(string jobTitle)
        {
            return (string[])db.BuildingAssign.Where(b => b.JobTitleHall.Equals(jobTitle)).Select(b => b.BuildingCode).ToArray();
        }

        /*
         * Create a new RCI for a user
         * @params: buildingCode - indicates which dorm building
         *          roomNumber - indicates roomNumber for new RCI
         *          id - indicates student's id (Note: a null id parameter indicates a common area RCI
         * @return: id for newly generated RCI
         */
        public int GenerateOneRCIinDb(string buildingCode, string roomNumber, string id = null)
        {
            var newRCI = new Rci();
            newRCI.GordonID = id;
            newRCI.BuildingCode = buildingCode;
            newRCI.RoomNumber = roomNumber;
            newRCI.IsCurrent = true;
            newRCI.CreationDate = DateTime.Now;
            newRCI.SessionCode = GetCurrentSession();

            db.Rci.Add(newRCI);
            db.SaveChanges();

            return newRCI.RciID;

        }

        /*
         * Create RCI Components that are associated with a single RCI, according to room type
         * @params: rciId - the id of the RCI to associate with 
         *          roomType - string to indicate type of room, either "common area" or "dorm room" currently
         */ 
        public void AddRciComponents(int rciId, string roomType)
        {
            var componentNames = new List<string>();
            if (roomType.Equals("common area"))
            {
                componentNames.AddRange(new string[]{ "Carpet", "Couch", "Sink",
                    "Living room table", "Kitchen table", "Kitchen Chairs"});
            }
            else // for now, just generic dorm; will add more checks once we've determined which components go where
            {
                componentNames.AddRange(new string[] { "Bed", "Carpet", "Desk", "Desk Chair",
                    "Dresser", "Wall", "Wardrobe" });
                ;
            }
            foreach (var name in componentNames)
            {
                var newComponent = new RciComponent();
                newComponent.RciComponentName = name.ToString();
                newComponent.RciID = rciId;

                db.RciComponent.Add(newComponent);
                db.SaveChanges();
            }
        }

        /*
         * Check to see if an up-to-date RCI exists for a user
         */ 
        public bool CurrentRciExists(IEnumerable<HomeRciViewModel> RCIs, string currentBuildingCode, string roomNumber)
        {
            var RCIsForCurrentBuilding = RCIs.Where(m => m.BuildingCode == currentBuildingCode && m.RoomNumber == roomNumber);
            return RCIsForCurrentBuilding.Any();
        }

        /*
         * Get the current common area RCIs for an apartment
         * @params: apartmentNumber - the apartment's number
         *          building - the building where the apartment is located
         * @return: the common area, if any, that was found in the db
         */ 
        public IEnumerable<HomeRciViewModel> GetCommonAreaRci(string apartmentNumber, string building)
        {
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
                    LastName = "RCI"
                };
            return commonAreaRCIs;

        }

        /*
         * Generate a csv file of fines for rci's from current, according to buildings of RD
         * Right now this is just generating for all buildings. We could add specificity so that RD can choose 
         * which building to pull from.
         * CSV format: RoomNumber, BuildingCode, Name, id, DetailedReason, FineAmount 
         * 
         */
         public string GenerateFinesSpreadsheet(List<string> buildingCodes)
        {
            var currentSession = GetCurrentSession();
            var csvString = "Room Number,Building Code,Name,ID,Detailed Reason,Fine Amount\n";

            // ***** This does not handle common areas! *****
            // We should talk to MC about how he wants common area fine assignment to be handled in the system
            var fineQueries =
                from rci in db.Rci
                join component in db.RciComponent on rci.RciID equals component.RciID
                join fine in db.Fine on component.RciComponentID equals fine.RciComponentID
                join account in db.Account on fine.GordonID equals account.ID_NUM
                where buildingCodes.Contains(rci.BuildingCode) && rci.SessionCode.Equals(currentSession)
                select new
                {
                    RoomNumber = rci.RoomNumber,
                    BuildingCode = rci.BuildingCode,
                    FirstName = account.firstname,
                    LastName = account.lastname,
                    Id = rci.GordonID,
                    ComponentName = component.RciComponentName,
                    DetailedReason = fine.Reason,
                    FineAmount = fine.FineAmount
                };

            foreach (var fine in fineQueries)
            {
                csvString += fine.RoomNumber + ",";
                csvString += fine.BuildingCode + ",";
                csvString += fine.FirstName + " " + fine.LastName + ",";
                csvString += fine.Id + ",";
                if (fine.ComponentName != null)
                {
                    csvString += fine.ComponentName + ": " + fine.DetailedReason + ",";
                }
                else
                {
                    csvString += "Improper checkout: " + fine.DetailedReason + ",";
                }
                csvString += fine.FineAmount + "\n";
            }

            Debug.Write(csvString);
            return csvString;
        } 

        /*
         * Query the db for the current session, which will be the session with the most recent SESS_BEGN_DTE
         * @return: string that contains the code for the current session
         */
         public string GetCurrentSession()
        {
            var currentSessionCode = db.Session.OrderByDescending(m => m.SESS_BEGN_DTE).FirstOrDefault().SESS_CDE;
            return currentSessionCode;
        } 

    }
}