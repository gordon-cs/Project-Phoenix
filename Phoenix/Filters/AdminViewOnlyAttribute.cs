using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class AdminViewOnlyAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];
            var isAdmin = (role == "ADMIN");

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