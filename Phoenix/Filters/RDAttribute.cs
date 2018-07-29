using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class RDAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];
            var isRD = role == "RD";
            var isAdmin = role == "ADMIN";

            if (!(isRD || isAdmin))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Dashboard", action = "Index" }));
                return;
            }
        }
    }
}