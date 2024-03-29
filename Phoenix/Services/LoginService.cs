﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using Jose;
using Phoenix.Exceptions;
using Phoenix.DapperDal;
using Phoenix.DapperDal.Types;

namespace Phoenix.Services
{

    public class LoginService : ILoginService
    {
        private readonly IDatabaseDal Dal;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public LoginService(IDatabaseDal dal)
        {
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
                ContextOptions.Negotiate | ContextOptions.ServerBind | ContextOptions.SecureSocketLayer);
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
            Account account;

            try
            {
                account = this.Dal.FetchAccountByGordonId(id);
            }
            catch (UserNotFoundException e)
            {
                // Log and rethrow so that the controller uphill can handle this exception properly.
                logger.Error(e, $"Account for username={username}, id={id} was not found!.");

                throw;
            }

            var accountPermissions = GetAccountPermissions(account);

            var mostRecentRoomAssign = GetCurrentRoomAssign(id);

            string currentBuildingCode = null;

            string currentRoomNumber = null;

            // Most Recent room assign will be null for non-students and that's ok.
            if (mostRecentRoomAssign != null)
            {
                currentBuildingCode = mostRecentRoomAssign.BuildingCode.Trim();

                currentRoomNumber = mostRecentRoomAssign.RoomNumber.Trim();
            }

            var name = account.FirstName.Trim() + " " + account.LastName.Trim();

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
                {"admin", account.IsAdmin },
                {"role", accountPermissions.Role.ToString() },
                {"kingdom", accountPermissions.Kingdom },
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
        
        public AccountPermission GetAccountPermissions(Account account)
        {
            var permission = new AccountPermission();

            var buildingMap = this.Dal.FetchBuildingMap();

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
        public RoomAssignment GetCurrentRoomAssign(string id)
        {
            try
            {
                return this.Dal.FetchRoomAssignmentForId(id, this.Dal.FetchCurrentSession());
            }
            catch (RoomAssignNotFoundException)
            {
                // Some people won't have a room assignment e.g. RD, faculty and staff. This is OK.
                return null;
            }
            catch(Exception e)
            {
                logger.Error(e, $"Unexpected exception when getting current room Assignment!");

                throw;
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