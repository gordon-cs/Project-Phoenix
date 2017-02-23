using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Diagnostics;
using System.Data.SqlClient;

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
            return buildingRCIs.OrderBy(m => m.RoomNumber);
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
        public void AddRciComponents(int rciId, string roomType, string buildingCode)
        {
            var componentNames = new List<string>();
            var componentDescriptions = new List<string>();
            if (roomType.Equals("common area"))
            {
                if (buildingCode.Equals("BRO") || buildingCode.Equals("TAV"))
                {
                    componentNames.AddRange(new string[]{ "Sofa", "Chair", "Coffee table", "Carpet", "Repaint walls",
                        "Wireless router", "Closet", "Table", "Chairs", "Refrigerator", "Cabinets / countertops",
                        "Outlets", "Stove", "Floor", "Walls", "Lights", "Recycling basket", "Shower",
                        "Floor", "Cabinets", "Mirror", "Repaint walls", "Light", "Toilet", "Towel bar"});
                }
                else // Ferrin
                {
                    componentNames.AddRange(new string[]{ "Sofa", "Chair", "Coffee table", "Carpet", "Repaint walls",
                        "Closet", "Table", "Chairs", "Refrigerator", "Cabinets", "Outlets", "Stove", "Floor", "Walls",
                        "Lights", "Recycling basket", "Shower", "Floor", "Cabinets", "Mirror", "Repaint walls", "Light",
                        "Toilet", "Towel bar"});
                }
                
            }
            else // for now, just generic dorm; will add more checks once we've determined which components go where
            {
                if (buildingCode.Equals("FER") || buildingCode.Equals("DRE"))
                {
                    componentNames.AddRange(new string[] { "Beds", "Carpet", "Dresser", "Desks", "Door", "Electrical", "Mirror",
                        "Wall/ceiling", "Closets", "Wardrobe", "Recycling basket", "Outlets", "Window", "Room cleaning"});
                    componentDescriptions.AddRange(new string[] { "frames, mattress, cover, spring", "minor burn, median stain, carpet replacement",
                        "surface, drawer", "lamp/light/carrel, surface top, desk chair", "replace, doorstop, lock damage, lockset replacement, card access lock replacement",
                        "ceiling light, outlets/switches, thermostat - heat control", "broken, stickers/marks", "repaint walls, repaint ceiling, paint chip, repair hole",
                        "doors, towel rack", "", "", "telephone, computer", "glass, lock/crank/lever", "general, floor only, furnishings, walls/door/ceiling"});
                }
                else if (buildingCode.Equals("EVA") || buildingCode.Equals("WIL") || buildingCode.Equals("LEW"))
                {
                    componentNames.AddRange(new string[] { "Black divider", "Beds", "Book shelf", "Carpet", "Closets", "Desks", "Door",
                        "Electrical", "Mirror", "Recycling basket", "Wall/ceiling", "Outlets", "Window", "Room cleaning"});
                    componentDescriptions.AddRange(new string[] { "tack holes", "frames, mattress, cover", "over bed, over desk", "minor burn, medium stain, carpet replacement", "door, drawers", "lamp/light, surface top, desk chair, drawer",
                        "replace, doorstop, lock damage, lockset replacement", "ceiling light, outlets/switches, thermostat - heat control", "broken, stickers/marks", "", "repaint walls, repaint ceiling, paint chip",
                        "telephone, computer", "glass, lock/crank/lever, screen, shade", "general, floor only, furnishings, walls/door/ceiling"});
                }
                else if (buildingCode.Equals("NYL"))
                {
                    componentNames.AddRange(new string[] { "Beds", "Carpet", "Dresser", "Desks", "Door", "Electrical", "Mirror",
                        "Wall/ceiling", "Wardrobe", "Recycling basket", "Outlets", "Window", "Room cleaning"});
                    componentDescriptions.AddRange(new string[] { "frames, mattress, cover", "minor burn, medium stain, carpet replacement", "surface, drawer", "lamp/light/carrel, surface top, desk chair, drawer, keyboard tray",
                        "replace, door closer, lock damage, lockset replacement, card access lock replacement", "ceiling light, outlets/switches, thermostat - heat control, wireless router", "broken, stickers/marks",
                        "repaint walls, repaint ceiling, paint chip, repair hole", "door", "", "telephone, computer", "glass, lock/crank/lever, screen, shade, cord/pulley", "general, floor only, furnishings, walls/door/ceiling"});
                }
                else if (buildingCode.Equals("TAV") || buildingCode.Equals("BRO")) // this is not really for bromley but we don't have the one for bromley for now.
                {
                    componentNames.AddRange(new string[] { "Beds", "Carpet", "Closets", "Desks", "Door", "Dresser", "Electrical",
                        "Mirror", "Wall/ceiling", "Outlets", "Window", "Baseboard heat cover", "Room cleaning"});
                    componentDescriptions.AddRange(new string[] { "frames, mattress, cover", "minor burn, medium stain, carpet replacement", "door, shelves", "lamp/light, surface top, desk chair, computer drawer",
                        "replace, lock damage, lockset replacement, card access lock replacement", "surface, drawer", "ceiling light, outlets/switches, thermostat - heat control", "broken, stickers/marks",
                        "repaint walls, repaint ceiling, repair small hole", "telephone, computer", "glass, lock, mini-blinds, screen", "", "general, floor only, furnishings, walls/door/ceiling"});
                }
                else // road halls. right now also chase and fulton as we don't have data for them
                {
                    componentNames.AddRange(new string[] { "Beds", "Bookshelf", "Carpet", "Closets", "Desks", "Door", "Dresser",
                        "Electrical", "Mirror", "Wall/ceiling", "Outlets", "Window", "Room cleaning"});
                    componentDescriptions.AddRange(new string[] { "frames, mattress, cover", "over desk", "minor burn, medium stain, carpet replacement", "door, tower rack", "lamp/light, surface top, desk chair, drawer",
                        "replace, lock damage, lockset replacement", "surface, drawer", "ceiling light, outlets/switches, thermostat - heat control", "broken, stickers/marks",
                        "repaint walls, repaint ceiling, paint chip, repair small hole", "", "telephone, computer", "glass, lock, shade, screen", "general, floor only, furnishings, walls/door/ceiling"});
                }
            }
            for (int i = 0; i < componentNames.Count(); i++)
            {
                var newComponent = new RciComponent();
                newComponent.RciComponentName = componentNames.ElementAt(i).ToString();
                if (roomType == "dorm room")
                    newComponent.RciComponentDescription = componentDescriptions.ElementAt(i).ToString();
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
            var currentSessionCode = db.Session.OrderByDescending(m => m.SESS_BEGN_DTE).FirstOrDefault().SESS_CDE.Trim();
            return currentSessionCode;
        }

        /// <summary>
        /// Create rci records that correspond with roomassign records for the list of buildings we are given
        /// </summary>
        public void SyncRoomRcis(List<string> kingdom)
        {
            var result = Enumerable.Empty<RoomAssign>();
            var currentSession = GetCurrentSession();

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

            var newRcis = new List<Rci>();

            foreach (var roomAssignment in result)
            {
                var newRci = new Rci
                {
                    IsCurrent = true,
                    BuildingCode = roomAssignment.BLDG_CDE.Trim(),
                    RoomNumber = roomAssignment.ROOM_CDE.Trim(),
                    CreationDate = DateTime.Now,
                    GordonID = roomAssignment.ID_NUM.ToString()
                };
                newRcis.Add(newRci);
            }

            db.Rci.AddRange(newRcis);

            db.SaveChanges();
        }

        /// <summary>
        /// Create rci records that correspond to common areas in the Room table for the list of buildings we are given.
        /// </summary>
        public void SyncCommonAreaRcis(List<string> kingdom)
        {
            var query =
                from rm in db.Room
                join rci in db.Rci
                on new { buildingCode = rm.BLDG_CDE.Trim(), roomNumber = rm.ROOM_CDE.Trim() } equals new { buildingCode = rci.BuildingCode, roomNumber = rci.RoomNumber } into matchedRcis
                from temp in matchedRcis.DefaultIfEmpty()
                where rm.MAX_CAPACITY == 0
                    && rm.ROOM_GENDER == null
                    && temp == null
                    && kingdom.Contains(rm.BLDG_CDE.Trim())
                select rm;
            var newCommonAreaRcis = new List<Rci>();

            foreach(var room in query)
            {
                var newCommonAreaRci = new Rci
                {
                    IsCurrent = true,
                    BuildingCode = room.BLDG_CDE.Trim(),
                    RoomNumber = room.ROOM_CDE.Trim(),
                    CreationDate = DateTime.Now,
                };
                newCommonAreaRcis.Add(newCommonAreaRci);
            }

            db.Rci.AddRange(newCommonAreaRcis);

            db.SaveChanges();
        }
    }
}