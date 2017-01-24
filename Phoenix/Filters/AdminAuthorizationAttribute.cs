using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Web.Mvc.Filters;
using System.Diagnostics;

namespace Phoenix.Filters
{
    public class AdminAuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var isAdmin = filterContext.Controller.TempData["admin"];
            // String comparison doesn't seem as good as bool comparison, but I wasn't sure how 
            // to parse out a bool from the decoded JSON object
           if (isAdmin.Equals("False"))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

           Debug.WriteLine("User is admin");
        }

    }
}