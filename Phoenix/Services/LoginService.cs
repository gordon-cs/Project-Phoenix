using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using Jose;
using Phoenix.Models;
using Phoenix.Exceptions;
using Phoenix.DapperDal;

namespace Phoenix.Services
{

    public class LoginService : ILoginService
    {
        private RCIContext db;

        private IDal Dal;

        public LoginService(IDal dal)
        {
            db = new Models.RCIContext();

            this.Dal = dal;
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
                "DC=gordon,DC=edu",
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
                throw new ArgumentNullException("One of the passed in arguments (username, ADContext) is null.");
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

            if (userEntry == null)
            {
                throw new UserNotFoundException(username);
            }

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

        public string GenerateToken(string username, string id)
        {
            // Add some code here to check the db to see if user has admin permissions
            // Right now just hardcode to true for test purposes
            bool isAdmin = false;

            // Will throw an exception if the user has no role within the system. The exceptions is caught in the controller.
            var accountPermissions = GetAccountPermissions(id);
            var mostRecentRoomAssign = GetCurrentRoomAssign(id);

            string currentBuildingCode = null;
            string currentRoomNumber = null;
            DateTime? currentRoomAssignDate = null;

            if (mostRecentRoomAssign != null)
            {
                currentBuildingCode = mostRecentRoomAssign.BLDG_CDE.Trim();
                currentRoomNumber = mostRecentRoomAssign.ROOM_CDE.Trim();
                currentRoomAssignDate = mostRecentRoomAssign.ASSIGN_DTE;
            }
            
            var account = db.Account.Where(m => m.ID_NUM == id).FirstOrDefault();
            var name = account.firstname + " " + account.lastname;

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
                {"role", accountPermissions.Role.ToString() },
                {"kingdom", accountPermissions.Kingdom },
                {"currentRoom", currentRoomNumber},
                {"currentBuilding",  currentBuildingCode },
                {"currentRoomAssignDate", currentRoomAssignDate }

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
        
        public AccountPermission GetAccountPermissions(string id)
        {
            var permission = new AccountPermission();

            if (id == null)
            {
                throw new ArgumentNullException("Can't get the role of a user with a null id number.");
            }

            Phoenix.DapperDal.Types.Account account = null;

            var buildingMap = this.Dal.FetchBuildingMap();

            try
            {
                account = this.Dal.FetchAccountByGordonId(id);
            }
            catch (Exception e)
            {
                throw new InvalidUserException($"User with ID {id} does not have a role within the system. {e.Message}");
            }

            if (account.IsAdmin)
            {
                permission.Role = Role.ADMIN;

                permission.Kingdom = this.Dal.FetchBuildingCodes();

                return permission;
            }
            else if (account.IsRd)
            {
                permission.Role = Role.RD;

                var hallgroup = buildingMap.Where(x => x.HallGroup.Equals(account.RdHallGroup)).FirstOrDefault();

                if (hallgroup == null)
                {
                    permission.Kingdom = new List<string>();
                }
                else
                {
                    permission.Kingdom = hallgroup.BuildingCodes;
                }

                return permission;
            }
            else if (account.IsRa)
            {
                permission.Role = Role.RA;

                var hallgroup = buildingMap.Where(x => x.BuildingCodes.Contains(account.RaBuildingCode)).FirstOrDefault();

                if (hallgroup == null)
                {
                    permission.Kingdom = new List<string>();
                }
                else
                {
                    permission.Kingdom = hallgroup.BuildingCodes;
                }

                return permission;
            }
            else
            {
                permission.Role = Role.Resident;
                permission.Kingdom = null;

                return permission;
            }
        }

        /* Get the most recent room assign information for the person  */
        public RoomAssign GetCurrentRoomAssign(string id)
        {
            var ResidentEntry = db.RoomAssign.Where(m => m.ID_NUM.ToString() == id).OrderByDescending(m => m.ASSIGN_DTE).FirstOrDefault();

            if (ResidentEntry != null)
            {
                return ResidentEntry;
            }
            else
            {
                // It is ok to return null here because there are people without room assignments that should
                // be able to log into the system e.g. RDs
                return null;
            }

        }

    }

    public class AccountPermission
    {
        public Role Role { get; set; }

        public List<string> Kingdom { get; set; } = new List<string>();
    }

    public enum Role { ADMIN, RA, RD, Resident }
}