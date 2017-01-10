using System.Web;
using System.Web.Mvc;
using Phoenix.Filters;

namespace Phoenix
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
           // filters.Add(new CustomAuthenticationAttribute());
        }
    }
}
