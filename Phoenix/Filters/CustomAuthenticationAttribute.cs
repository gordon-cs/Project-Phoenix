using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Jose;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Phoenix.Filters
{
    public class CustomAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            Debug.WriteLine("Reached OnAuthentication method");
            
            HttpCookie authCookie = filterContext.HttpContext.Request.Cookies["Authentication"];
            if (authCookie != null)
            {
                string jwtToken = authCookie.Value;
                var secretKey = new byte[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
                string decodedString = Jose.JWT.Decode(jwtToken, secretKey);
                Debug.WriteLine(decodedString);
                JObject decodedJson = JObject.Parse(decodedString);
                // if it was decoded into something meaningful, the "sub" key will have the username
                if (decodedJson["sub"] == null)
                {
                    // user has not been authenticated, so they must be redirected
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
            else
            {
                // user has not been authenticated, so they must be redirected
                filterContext.Result = new HttpUnauthorizedResult();
            }

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            Debug.WriteLine("Reached OnAuthenticationChallenge method");
        }
    }
}