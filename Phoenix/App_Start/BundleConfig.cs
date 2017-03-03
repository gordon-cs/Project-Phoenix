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
                        "~/Scripts/logout.js",
                        "~/Scripts/dom-manipulation.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/css/default-styles.css",
                      "~/css/site-layout.css",
                      "~/css/site-colors.css",
                      "~/css/site-typography.css"));

            // Page specific css
            bundles.Add(new StyleBundle("~/bundles/page_specific/css/landing_page").Include(
                        "~/css/under_construction.css"));

            bundles.Add(new StyleBundle("~/bundles/page_specific/css/login").Include(
                        "~/css/login.css"));

            bundles.Add(new StyleBundle("~/bundles/page_specific/css/dashboard").Include(
                        "~/css/dashboard.css"));

            bundles.Add(new StyleBundle("~/bundles/page_specific/css/rci-input").Include(
                        "~/css/rci-input.css"));

            bundles.Add(new StyleBundle("~/bundles/page_specific/css/rci-checkout").Include(
                        "~/css/rci-checkout.css"));
            bundles.Add(new StyleBundle("~/bundles/page_specific/css/rci-component-reassign").Include(
                        "~/css/rci-component-reassign.css"));

            // Page specific js
            bundles.Add(new ScriptBundle("~/bundles/page_specific/js/landing_page").Include(
                        "~/Scripts/countdown_timer.js"));

            bundles.Add(new ScriptBundle("~/bundles/page_specific/js/rci-input").Include(
                        "~/Scripts/rci-input.js"));

            bundles.Add(new ScriptBundle("~/bundles/page_specific/js/rci-checkout").Include(
                        "~/Scripts/rci-checkout.js"));

            bundles.Add(new ScriptBundle("~/bundles/page_specific/js/modal").Include(
                        "~/Scripts/modal.js"));

            //Uncomment the line below to start minifying and bundling.
            //BundleTable.EnableOptimizations = true;
        }
    }
}
