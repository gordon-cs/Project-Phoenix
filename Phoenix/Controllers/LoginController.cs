using System;
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
        public ActionResult Index()
        {
            if (TempData["ErrorMsg"] != null)
            {
                ViewBag.LoginError = TempData["ErrorMsg"].ToString();
            }
            return View();
        }

        /*
         * Method for authenticating. If user is validated, redirect to the RCI Dashboard. 
         * If user doesn't validate for any reason, redirect back to Login page
         */ 
         [HttpPost]
        public ActionResult Authenticate(LoginViewModel loginViewModel)
        {

            // Get the username and password from the view model
            string username = loginViewModel.username;
            string password = loginViewModel.password;

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Invalid model state.");
                // Not sure what kind of user response we should give here
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    ConnectToADServer();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception caught: ", e.ToString());
                    Debug.WriteLine("LDAP Connection Error","Error connecting to LDAP server");
                    //context.SetError("Connection_error",
                    //    "There was a problem connecting to the Active Directory LDAP server.");
                    TempData["ErrorMsg"] = "This is awkward... We had a problem connecting to the server. We're so sorry,"+ 
                        "but you may have to try again later or contact a system administrator.";
                    return RedirectToAction("Index");
                }
                if (_ADContext != null)
                {
                    var userEntry = FindUser(username);
                    if (userEntry == null)
                    {
                        TempData["ErrorMsg"] = "Oh dear, it seems that username does not exist in the database.";
                        _ADContext.Dispose();
                        return RedirectToAction("Index");
                    }
                    // If user does exist in Active Directory, try to validate him or her
                    else
                    {
                        // Establish a new connection to server, which will be needed to validate user. Not ENTIRELY sure why
                        ConnectToADServer();
                        if (IsValidUser(username, password))
                        {
                            TempData["Name"] = userEntry.Name;
                            
                            // I think we could add code here for authorization of admin, etc.

                            // Generate token and attach to header
                            var jwtToken = GenerateToken(username);
                            HttpCookie tokenCookie = new HttpCookie("Authentication");
                            tokenCookie.Value = jwtToken;
                            Response.Cookies.Add(tokenCookie);

                            _ADContext.Dispose();
                            // Once dashboard is implemented, redirect there. For now, go to placeholder
                            return Redirect("/Placeholder");
                        }
                        else
                        {
                            Debug.WriteLine("Oops, the username or password is incorrect.");
                            //context.SetError("Invalid_grant", "The username or password is incorrect.");

                            // If user is not valid, redirect to the Login Page
                            TempData["ErrorMsg"] = "Oops, the username or password is incorrect.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index");
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
         //Test
        public bool IsValidUser(string username, string password)
        {
            return _ADContext.ValidateCredentials(
                username,
                password,
                ContextOptions.SimpleBind | ContextOptions.SecureSocketLayer);
        }

        public string GenerateToken(string username)
        {
            // ****** THIS NEEDS TO BE CHANGED I THINK. NOT VERY SECURE **********
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

        // Set principal
    }
}