using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class ResLifeStaffViewOnlyAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];
            var isRD = role == "RD";
            var isRA = role == "RA";
            var isAdmin = role == "ADMIN";

            if (!(isRD || isRA || isAdmin)) // Anythin other than a member of the residence life staff
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Dashboard", action = "Index" }));
                return;
            }
        }
    }
}