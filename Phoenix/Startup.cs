using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using Phoenix.Controllers.Authentication;

[assembly: OwinStartup(typeof(Phoenix.Startup))]
namespace Phoenix
{
    /*
     * Owin's startup mechanism
     * Replaces the Global.asax file
     * Added according to Step 3 in this tutorial: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
     * Modified for MVC according to this article: http://edgamat.com/2014/06/using-owin-to-build-a-clean-asp-net-mvc-startup/
     */
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureOAuth(app);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = 
                new OAuthAuthorizationServerOptions()
                {
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new PathString("/token"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                    Provider = new RCIAuthorizationServerProvider()
                };
        }
    }
}