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
           
        }

    }
}