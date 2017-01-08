using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Diagnostics;

namespace Phoenix.Controllers.Authentication
{
    /* Authentication mechanism
     * Referenced Taiseer Joudeh's blog: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
     * and Eze Anyanwu's work on Gordon360 https://github.com/gordon-cs/Project-Raymond/blob/master/Gordon360/AuthorizationServer/TokenIssuer.cs
     */

    public class RCIAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        // Global variables

        PrincipalContext _ADContext;

        // Represents the user entry, if found, in the Active Directory on the LDAP server
        public UserPrincipal _userEntry;

        public bool _userValidated = false;

        // I think this variable could be used for authorization purposes
        public ClaimsIdentity _userIdentity;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // We only have one client, so we always return that it's validated successfully
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            try
            {
                ConnectToADServer();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception caught: ", e.ToString());
                context.SetError("Connection_error",
                    "There was a problem connecting to the Active Directory LDAP server.");
            }
            if (_ADContext != null)
            {
                var userEntry = FindUser(context);
                if (userEntry == null)
                {
                    context.SetError("Unsuccessful_Login", "Username does not exist in database.");
                    _ADContext.Dispose();
                }
                // If user does exist in Active Directory, try to validate him or her
                else
                {
                    if(isValidUser(context))
                    {
                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                        identity.AddClaim(new Claim("name", userEntry.Name));
                        // I think we could add code here for authorization of admin, etc.

                        _ADContext.Dispose();
                        context.Validated(identity);
                    }
                    else
                    {
                        context.SetError("Invalid_grant", "The username or password is incorrect.");
                    }
                }
            }
        }

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
        public UserPrincipal FindUser(OAuthGrantResourceOwnerCredentialsContext oauthContext)
        {
            // Create a UserPrincipal object, with the entered username, to be used as a filter with which to query the Active Directory 
            UserPrincipal userQueryFilter = new UserPrincipal(_ADContext);
            userQueryFilter.SamAccountName = oauthContext.UserName;

            // Now set up a searcher with appropriate query, to query AD
            PrincipalSearcher searcher = new PrincipalSearcher(userQueryFilter);
            UserPrincipal userEntry = (UserPrincipal)searcher.FindOne();
            searcher.Dispose();

            return userEntry;
        }

        /*
         * Validate a user
         */ 
         public bool isValidUser( OAuthGrantResourceOwnerCredentialsContext oauthContext)
        {
            return _ADContext.ValidateCredentials(
                oauthContext.UserName,
                oauthContext.Password,
                ContextOptions.SecureSocketLayer);
        }
    }
}