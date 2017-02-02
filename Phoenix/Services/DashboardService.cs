using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Phoenix.Models;
using Phoenix.Models.ViewModels;

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
        public IEnumerable<HomeRCIViewModel> GetRCIsForResident(string id)
        {
            var RCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                where account.ID_NUM == id && personalRCI.Current == true
                select new HomeRCIViewModel
                {
                    RCIID = personalRCI.RCIID,
                    BuildingCode = personalRCI.BuildingCode,
                    RoomNumber = personalRCI.RoomNumber,
                    FirstName = account.firstname,
                    LastName = account.lastname
                };
            return RCIs;
        }

        /*
         * Display all RCI's for the corresponding building 
         */
        public IEnumerable<HomeRCIViewModel> GetRCIsForBuilding(string[] buildingCode)
        {
            // Not sure if this will end up with duplicates for the RA's own RCI
            var buildingRCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where buildingCode.Contains(personalRCI.BuildingCode) && personalRCI.Current == true
                select new HomeRCIViewModel
                {
                    RCIID = personalRCI.RCIID,
                    BuildingCode = personalRCI.BuildingCode,
                    RoomNumber = personalRCI.RoomNumber,
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
        public string [] CollectRDBuildingCodes(string buildingCode)
        {
            return (string[])db.BuildingAssign.Where(b => b.Job_Title_Hall.Equals(buildingCode)).Select(b => b.BuildingCode).ToArray();
        }

        /*
         * Create a new RCI for a user
         * @params: buildingCode - indicates which dorm building
         *          roomNumber - indicates roomNumber for new RCI
         *          id - indicates student's id (Note: a null id parameter indicates a common area RCI
         * @return: id for newly generated RCI
         */
        public int GenerateOneRCI(string buildingCode, string roomNumber, string id = null)
        {
            var newRCI = new RCI();
            newRCI.GordonID = id;
            newRCI.BuildingCode = buildingCode;
            newRCI.RoomNumber = roomNumber;
            newRCI.Current = true;
            newRCI.CreationDate = DateTime.Now;

            db.RCI.Add(newRCI);
            db.SaveChanges();

            return newRCI.RCIID;

        }

        /*
         * Create RCI Components that are associated with a single RCI, according to room type
         * @params: rciId - the id of the RCI to associate with 
         *          roomType - string to indicate type of room, either "common area" or "dorm room" currently
         */ 
        public void AddRCIComponents(int rciId, string roomType)
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
                var newComponent = new RCIComponent();
                newComponent.RCIComponentName = name.ToString();
                newComponent.RCIID = rciId;

                db.RCIComponent.Add(newComponent);
                db.SaveChanges();
            }
        }

        /*
         * Check to see if an up-to-date RCI exists for a user
         */ 
        public bool IndividualCorrectCurrentRCIExists(IEnumerable<HomeRCIViewModel> RCIs, string building, string roomNumber)
        {
            var RCIsForCurrentBuilding = RCIs.Where(m => m.BuildingCode == building && m.RoomNumber == roomNumber);
            return RCIsForCurrentBuilding.Any();
        }

        /*
         * Verifies that all individual and common area RCIs which SHOULD exists, do exist
         * Return: updated collection of HomeRCIViewModels
         */
        public IEnumerable<HomeRCIViewModel> ValidateResidentsRCIsExistence(IEnumerable<HomeRCIViewModel> RCIs, string building, string roomNumber, string id)
        {
            if (!IndividualCorrectCurrentRCIExists(RCIs, building, roomNumber))
            {
                var rciId = GenerateOneRCI(building, roomNumber, id);
                AddRCIComponents(rciId, "dorm room");
            }

            if (building.Equals("BRO") || building.Equals("TAV") ||
                (building.Equals("FER") && (roomNumber.StartsWith("L"))))
            {

                roomNumber = roomNumber.TrimEnd(new char[] { 'A', 'B', 'C', 'D' });
                var commonAreaRCIs =
                    from tempCommonAreaRCI in db.RCI
                    where tempCommonAreaRCI.RoomNumber == roomNumber && tempCommonAreaRCI.BuildingCode == building
                    && tempCommonAreaRCI.GordonID == null && tempCommonAreaRCI.Current == true
                    select new HomeRCIViewModel
                    {
                        RCIID = tempCommonAreaRCI.RCIID,
                        BuildingCode = tempCommonAreaRCI.BuildingCode,
                        RoomNumber = tempCommonAreaRCI.RoomNumber,
                        FirstName = "Common",
                        LastName = "RCI"
                    };

                // If there was no common area RCI for someone in BRO, TAV, or FER apts, then add one
                if (!commonAreaRCIs.Any())
                {

                    var rciId = GenerateOneRCI(building, roomNumber);
                    AddRCIComponents(rciId, "common area");
                }

                RCIs = RCIs.Concat(commonAreaRCIs);
            }

            return RCIs;
        }

    }
}