using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using Jose;
using Phoenix.Models;

namespace Phoenix.Services
{

    public class LoginService
    {
        private RCIContext db;

        public LoginService()
        {
            db = new Models.RCIContext();
        }

        /*
         * Connect to Gordon's LDAP server where Active Directory of users is stored
         * Return: PrincipalContext - a container that encapuslates the server connection
         */
        public PrincipalContext ConnectToADServer()
        {
            PrincipalContext ADContext = new PrincipalContext(
                ContextType.Domain,
                "elder.gordon.edu:636",
                "OU=Gordon College,DC=gordon,DC=edu",
                ContextOptions.Negotiate | ContextOptions.ServerBind | ContextOptions.SecureSocketLayer,
                "CS-LDAP-CCT",
                "QUl59QpdpL**sTwZ");
            // This password should probs be stored elsewhere

            return ADContext;
        }

        /*
         * Query the Active Directory db to see if user exists
         */
        public UserPrincipal FindUser(string username, PrincipalContext ADContext)
        {
            if(username == null || ADContext == null)
            {
                return null;
            }

            if (username.EndsWith("@gordon.edu"))
            {
                username = username.Remove(username.IndexOf('@'));
            }
            // Create a UserPrincipal object, with the entered username, to be used as a filter with which to query the Active Directory 
            UserPrincipal userQueryFilter = new UserPrincipal(ADContext);
            userQueryFilter.SamAccountName = username;

            // Now set up a searcher with appropriate query, to query AD
            PrincipalSearcher searcher = new PrincipalSearcher(userQueryFilter);
            UserPrincipal userEntry = (UserPrincipal)searcher.FindOne();
            searcher.Dispose();

            return userEntry;
        }

        /*
         * Validate a user
         */
        public bool IsValidUser(string username, string password, PrincipalContext ADContext)
        {
            if(username == null || password == null || ADContext == null)
            {
                return false;
            }
            return ADContext.ValidateCredentials(
                username,
                password,
                ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer);
        }

        public string GenerateToken(string username, string name, string id)
        {
            var role = GetRole(id);
            var currentBuildingCode = GetCurrentBuilding(id);
            var currentRoomNumber = GetCurrentRoom(id);
            var kingdom = GetKingdom(id); // The buildings for which you are responsible

            // ****** THIS NEEDS TO BE CHANGED. NOT VERY SECURE **********
            var secretKey = new byte[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

            DateTime issued = DateTime.Now;
            DateTime expire = DateTime.Now.AddHours(2);
            var payload = new Dictionary<string, object>()
            {
                {"sub", username  },
                {"name", name },
                {"gordonId", id },
                {"iss", "rci.gordon.edu" },
                {"iat", ToUnixTime(issued) },
                {"exp", ToUnixTime(expire) },
                {"role", role },
                {"kingdom", kingdom },
                {"currentRoom", currentRoomNumber},
                {"currentBuilding",  currentBuildingCode }
            };

            string token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
            return token;
        }

        // Geneerate unix timestamp
        public long ToUnixTime(DateTime dateTime)
        {
            return (int)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /*
         * Get the role of a user
         */
        public string GetRole(string id)
        {
            if(id == null)
            {
                return null;
            }
            var AdminEntry = db.Admin.Where(m => m.GordonID == id);
            if (AdminEntry != null)
            {
                return "ADMIN";
            }
            var RDentry = db.CurrentRD.Where(m => m.ID_NUM == id).FirstOrDefault();
            if (RDentry != null)
            {
                return "RD";
            }
            var RAentry = db.CurrentRA.Where(m => m.ID_NUM.ToString() == id).FirstOrDefault();
            if (RAentry != null)
            {
                return "RA";
            }
            return "Resident";

        }

        /*
         * Get a list of building codes that this user is responsible for.
         */
        public List<string> GetKingdom (string id)
        {
            if (id == null)
            {
                return null;
            }
            var AdminEntry = db.Admin.Find(id);
            if(AdminEntry != null)
            {
                var buildingCodes = db.BuildingAssign.Select(x => x.BuildingCode).ToList();
                return buildingCodes;
            }


            var RDentry = db.CurrentRD.Where(m => m.ID_NUM == id).FirstOrDefault();
            if (RDentry != null)
            {
                // Get the building codes associated with the RD's Job Title
                var buildingCodes = db.BuildingAssign.Where(m => m.JobTitleHall.Equals(RDentry.Job_Title_Hall)).Select(m => m.BuildingCode).ToList();
                return buildingCodes;
            }

            var RAentry = db.CurrentRA.Where(m => m.ID_NUM.ToString() == id).FirstOrDefault();
            if (RAentry != null)
            {
                // Since CurrentRA has building codes, we need to a do a little kung fu to make sure cases like
                // the road halls work.

                // Step 1: For a given buildingCode, get what JobTitle it is associated with.
                var temp = db.BuildingAssign.Where(m => m.BuildingCode.Equals(RAentry.Dorm)).FirstOrDefault();
                var raJobTitleHall = temp.JobTitleHall;

                // Step 2: Now that we have the job title, get what building codes it is associated with.
                var buildingCodes = db.BuildingAssign.Where(m => m.JobTitleHall.Equals(raJobTitleHall)).Select(m => m.BuildingCode).ToList();
                return buildingCodes;
            }

            // If you are not an RA or RD, you don't have a kingdom  :p
            return null;

        }

        /* Get the building where the person is currently living */
        public string GetCurrentBuilding(string id)
        {
            var ResidentEntry = db.RoomAssign.Where(m => m.ID_NUM.ToString() == id).OrderByDescending(m => m.ASSIGN_DTE).FirstOrDefault();
            if(ResidentEntry != null)
            {
                return ResidentEntry.BLDG_CDE.Trim();
            }
            return null;
        }

        /* Get the room number where the person is currently living */
        public string GetCurrentRoom(string id)
        {
            var ResidentEntry = db.RoomAssign.Where(m => m.ID_NUM.ToString() == id).OrderByDescending(m => m.ASSIGN_DTE).FirstOrDefault();
            if (ResidentEntry != null)
            {
                return ResidentEntry.ROOM_CDE.Trim();
            }
            return null;
        }
    }
}