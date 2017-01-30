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

            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap").Include(
                      "~/Scripts/third_party/bootstrap.js",
                      "~/Scripts/third_party/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/rci").Include(
                        "~/Scripts/rci-input.js",
                        "~/Scripts/countdown_timer.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/css/third_party/bootstrap.css",
                      "~/css/site.css",
                      "~/css/under_construction.css",
                      "~/css/login.css"));

            //Uncomment the line below to start minifying and bundling.
            //BundleTable.EnableOptimizations = true;
        }
    }
}
