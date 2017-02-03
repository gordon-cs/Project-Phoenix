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
            return ADContext.ValidateCredentials(
                username,
                password,
                ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer);
        }

        public string GenerateToken(string username, string name, string id)
        {
            // Add some code here to check the db to see if user has admin permissions
            // Right now just hardcode to true for test purposes
            bool isAdmin = false;


            var role = GetRole(id);
            var building = GetBuilding(id);
            var roomNumber = GetRoom(id);

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
                {"admin", isAdmin },
                {"role", role },
                {"building", building },
                {"room", roomNumber}
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
         * Get the building a user lives in.
         */
        public string GetBuilding(string id)
        {
            var RDentry = db.CurrentRD.Where(m => m.ID_NUM == id).FirstOrDefault();
            if (RDentry != null)
            {
                return RDentry.Job_Title_Hall;
            }
            var RAentry = db.CurrentRA.Where(m => m.ID_NUM.ToString() == id).FirstOrDefault();
            if (RAentry != null)
            {
                return RAentry.Dorm;
            }
            var ResidentEntry = db.RoomAssign.Where(m => m.ID_NUM.ToString() == id).OrderByDescending(m => m.ASSIGN_DTE).FirstOrDefault();
            if (ResidentEntry != null)
            {
                return ResidentEntry.BLDG_CDE;
            }
            return "Non-Resident";

        }

        public string GetRoom(string id)
        {
            var ResidentEntry = db.RoomAssign.Where(m => m.ID_NUM.ToString() == id).OrderByDescending(m => m.ASSIGN_DTE).FirstOrDefault();
            if (ResidentEntry != null)
            {
                return ResidentEntry.ROOM_CDE;
            }
            return "Non-Resident";
        }
    }
}