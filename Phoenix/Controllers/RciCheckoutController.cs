using Phoenix.Filters;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RciCheckoutController : Controller
    {
        private RciCheckoutService checkoutService;

        public RciCheckoutController()
        {
            checkoutService = new RciCheckoutService();
        }

        // GET: RCICheckout
        /// <summary>
        /// Dislay the checkout view for the specified rci
        /// </summary>
        /// <param name="id">RCI identifier</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            // Redirect to other dashboards if role not correct
            var role = (string)TempData["role"];
            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var rci = checkoutService.GetRciByID(id);

            return View(rci);
        }

        public void SaveRci(RciFinesForm rci)
        {
            checkoutService.AddFines(rci.NewFines, rci.GordonID);
            checkoutService.RemoveFines(rci.FinesToDelete);

            return;
        }

        public ActionResult ResidentSignature(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var rci = checkoutService.GetRciByID(id);
            return View(rci);
        }

        public ActionResult RASignature(int id)
        {
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];

            if (role == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (role.Equals("Resident"))
            {
                return RedirectToAction("ResidentSignature", new { id = id });
            }

            var rci = checkoutService.GetRciByID(id);
            return View(rci);
        }
    }
}