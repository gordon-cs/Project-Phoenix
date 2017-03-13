using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class ResLifeStaffAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];
            var isRD = role == "RD";
            var isRA = role == "RA";
            var isAdmin = role == "ADMIN";

            if (!(isRD || isRA || isAdmin))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Dashboard", action = "Index" }));
                return;
            }
        }
    }
}