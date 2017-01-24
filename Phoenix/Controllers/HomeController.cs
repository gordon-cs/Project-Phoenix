using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class HomeController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private RCIContext db;

        public HomeController()
        {
            db = new Models.RCIContext();
        }

        // GET: Home
        public ActionResult Index()
        {

            var role = TempData["role"];
            if(role.Equals("RD"))
            {
                return RedirectToAction("RD");
            }
            else if (role.Equals("RA"))
            {
                return RedirectToAction("RA");
            }
            else
            {

                return RedirectToAction("Resident" );
            }
          
        }

        // GET: Home/Resident
        public ActionResult Resident()
        {
            // Look through RCIS and find your RCI with your ID
            // For common area RCI, look through rci's without a gordon id, 
            // with the corresponding Building and Room number
            var personalRCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                where account.ID_NUM.Equals(TempData["id"])
                select new HomeRCIViewModel{ BuildingCode = personalRCI.BuildingCode, RoomNumber = personalRCI.RoomNumber, FirstName = account.firstname, LastName = account.lastname};
            return View(personalRCIs);
        }

        // GET: Home/RA
        public ActionResult RA()
        {
            // Display all RCI's for the corresponding building

            var personalRCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                select new HomeRCIViewModel { BuildingCode = personalRCI.BuildingCode, RoomNumber = personalRCI.RoomNumber, FirstName = account.firstname, LastName = account.lastname };
            return View(personalRCIs);
        }

        // GET: Home/RD
        public ActionResult RD()
        {
            // Display all RCI's for the corresponding building

            var personalRCIs =
                 from personalRCI in db.RCI
                 join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                 select new HomeRCIViewModel { BuildingCode = personalRCI.BuildingCode, RoomNumber = personalRCI.RoomNumber, FirstName = account.firstname, LastName = account.lastname };
            return View(personalRCIs);
        }

        // Potentially later: admin option that can view all RCI's for all buildings
    }
}