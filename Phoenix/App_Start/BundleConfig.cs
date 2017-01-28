using System.Web;
using System.Web.Optimization;

namespace Phoenix
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js/jquery").Include(
                        "~/Scripts/third_party/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jqueryval").Include(
                        "~/Scripts/third_party/jquery.validate*"));


            bundles.Add(new ScriptBundle("~/bundles/js/rci").Include(
                        "~/Scripts/rci-input.js",
                        "~/Scripts/countdown_timer.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/css/default-styles.css",
                      "~/css/site-layout.css",
                      "~/css/site-colors.css",
                      "~/css/site-typography.css",
                      "~/css/under_construction.css"));
            
            // Page specific css
            bundles.Add(new StyleBundle("~/bundles/page_specific/login").Include(
                        "~/css/login.css"));

            bundles.Add(new StyleBundle("~/bundles/page_specific/dashboard").Include(
                        "~/css/dashboard.css"));
            //Uncomment the line below to start minifying and bundling.
            //BundleTable.EnableOptimizations = true;
        }
    }
}
