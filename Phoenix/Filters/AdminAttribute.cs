using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Web.Mvc.Filters;
using System.Diagnostics;

namespace Phoenix.Filters
{
    public class AdminAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];

            var isAdmin = role == "ADMIN";
            // String comparison doesn't seem as good as bool comparison, but I wasn't sure how 
            // to parse out a bool from the decoded JSON object
           if (!isAdmin)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new System.Web.Routing.RouteValueDictionary(
                       new { controller = "Dashboard", action = "Index" }));
                return;
            }

        }

    }
}