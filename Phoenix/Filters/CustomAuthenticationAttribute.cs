﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace Phoenix.Filters
{
    public class CustomAuthenticationAttribute : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {            
            HttpCookie authCookie = filterContext.HttpContext.Request.Cookies["Authentication"];


            if (authCookie == null)
            {
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
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Login", action = "Index" }));
                return;
            }

            JObject decodedJson = JObject.Parse(decodedString);

            // Add key/value pairs that we might want to access in the controller
            filterContext.Controller.TempData["user"] = decodedJson["name"].ToString();
            if (decodedJson["admin"].ToString() != null)
            {
                filterContext.Controller.TempData["admin"] = decodedJson["admin"].ToString();
            }
            else
            {
                filterContext.Controller.TempData["admin"] = null;
            }
            filterContext.Controller.TempData["role"] = decodedJson["role"].ToString();
            filterContext.Controller.TempData["id"] = decodedJson["gordonId"].ToString();
            filterContext.Controller.TempData["currentBuilding"] = decodedJson["currentBuilding"].ToString();
            filterContext.Controller.TempData["currentRoom"] = decodedJson["currentRoom"].ToString();
            filterContext.Controller.TempData["kingdom"] = decodedJson["kingdom"];
            filterContext.Controller.TempData["login_username"] = decodedJson["sub"].ToString();           

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {}
    }
}