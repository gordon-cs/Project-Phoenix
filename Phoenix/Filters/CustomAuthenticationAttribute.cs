using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
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


            if (authCookie == null)
            {
                Debug.WriteLine("Authcookie is null");

                // Set the context result to redirect.
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Login", action = "Index" }));

                // Return so we don't continue
                return;
            }


            var jwtToken = authCookie.Value;
            var decodedString = "";
            var secretKey = new byte[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

            // Jose throws an exception on decode if the token isn't intact. #cryptographicmagic
            try
            {
                decodedString = Jose.JWT.Decode(jwtToken, secretKey);
            }
            catch(Exception)
            {
                Debug.WriteLine("There was an error decoding the token.");
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Login", action = "Index" }));
                return;
            }

            JObject decodedJson = JObject.Parse(decodedString);

            // Add key/value pairs that we might want to access in the controller
            filterContext.Controller.TempData["user"] = decodedJson["name"].ToString();
            filterContext.Controller.TempData["admin"] = decodedJson["admin"].ToString();
            filterContext.Controller.TempData["role"] = decodedJson["role"].ToString();
            filterContext.Controller.TempData["id"] = decodedJson["gordonId"].ToString();
            filterContext.Controller.TempData["currentBuilding"] = decodedJson["currentBuilding"].ToString();
            filterContext.Controller.TempData["currentRoom"] = decodedJson["currentRoom"].ToString();
            filterContext.Controller.TempData["currentRoomAssignDate"] = decodedJson["currentRoomAssignDate"];
            filterContext.Controller.TempData["kingdom"] = decodedJson["kingdom"];

           
            Debug.WriteLine(decodedJson.ToString());
           

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            Debug.WriteLine("Reached OnAuthenticationChallenge method");
        }
    }
}