﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Diagnostics;
using Phoenix.Models.ViewModels;
using Jose;

namespace Phoenix.Controllers
{
    public class LoginController : Controller
    {
        // Global Variables
        PrincipalContext _ADContext;

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
         [HttpPost]
        public ActionResult Index(LoginViewModel loginViewModel)
        {

            // Get the username and password from the view model
            string username = loginViewModel.username;
            string password = loginViewModel.password;

            if (!ModelState.IsValid)
            {
                loginViewModel.errorMessage = "Invalid model state.";
                return View(loginViewModel);
            }
            else
            {
                try
                {
                    ConnectToADServer();
                }
                catch (Exception e)
                {
                    loginViewModel.errorMessage = "There was a problem connection to the server. We are sorry. Please try again later or contact a system administrator.";
                    return View(loginViewModel);
                }
                if (_ADContext != null)
                {
                    var userEntry = FindUser(username);
                    if (userEntry == null)
                    {
                        _ADContext.Dispose();
                        loginViewModel.errorMessage = "Invalid Username or password.";
                        return View(loginViewModel);
                    }
                    // If user does exist in Active Directory, try to validate him or her
                    else
                    {
                        if (IsValidUser(username, password))
                        {

                            // I think we could add code here for authorization of admin, etc.

                            // Generate token and attach to header
                            var jwtToken = GenerateToken(username);
                            HttpCookie tokenCookie = new HttpCookie("Authentication");
                            tokenCookie.Value = jwtToken;
                            Response.Cookies.Add(tokenCookie);

                            _ADContext.Dispose();
                            // Once dashboard is implemented, redirect there. For now, go to placeholder
                            return RedirectToAction("Index", "RCIInput");
                        }
                        else
                        {
                            loginViewModel.errorMessage = "Invalid Username or password.";
                            return View(loginViewModel);
                        }
                    }
                }
                else
                {
                    loginViewModel.errorMessage = "Invalid Username or password.";
                    return View(loginViewModel);
                }
            }
        }

        // ******* Helper methods; not sure if these should be moved to a service **********

        /*
         * Connect to Gordon's LDAP server where Active Directory of users is stored
         * Return: PrincipalContext - a container that encapuslates the server connection
         */
        public void ConnectToADServer()
        {
            _ADContext = new PrincipalContext(
                ContextType.Domain,
                "elder.gordon.edu:636",
                "OU=Gordon College,DC=gordon,DC=edu",
                ContextOptions.Negotiate | ContextOptions.ServerBind | ContextOptions.SecureSocketLayer,
                "CS-LDAP-CCT",
                "QUl59QpdpL**sTwZ");
        }

        /*
         * Query the Active Directory db to see if user exists
         */
        public UserPrincipal FindUser(string username)
        {
            if (username.EndsWith("@gordon.edu"))
            {
                username = username.Remove(username.IndexOf('@'));
            }
            // Create a UserPrincipal object, with the entered username, to be used as a filter with which to query the Active Directory 
            UserPrincipal userQueryFilter = new UserPrincipal(_ADContext);
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
        public bool IsValidUser(string username, string password)
        {
            return _ADContext.ValidateCredentials(
                username,
                password,
                ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer);
        }

        public string GenerateToken(string username)
        {
            // ****** THIS NEEDS TO BE CHANGED. NOT VERY SECURE **********
            var secretKey = new byte[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

            DateTime issued = DateTime.Now;
            DateTime expire = DateTime.Now.AddHours(2);
            var payload = new Dictionary<string, object>()
            {
                {"sub", username  },
                {"iss", "rci.gordon.edu" },
                {"iat", ToUnixTime(issued) },
                {"exp", ToUnixTime(expire) }
            };

            string token = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
            return token;
        }

        // Geneerate unix timestamp
        public long ToUnixTime(DateTime dateTime)
        {
            return (int)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}