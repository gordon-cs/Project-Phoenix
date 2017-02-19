using System.Diagnostics;
using System.Web.Mvc;


namespace Phoenix.Filters
{
    /// <summary>
    /// Restrict this view to RDs only
    /// </summary>
    public class RDViewOnlyAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var role = (string)filterContext.Controller.TempData["role"];
            var isRD = role == "RD";

            if (!isRD)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Dashboard", action = "Index" }));
                return;
            }
        }
    }
}