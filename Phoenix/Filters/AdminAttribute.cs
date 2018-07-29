using Phoenix.Utilities;
using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class AdminAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];

            var isAdmin = role == Constants.ADMIN;
            
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