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
            var resRCIs =
                from resRCI in db.ResidentRCI
                join account in db.Account on resRCI.ResidentAccountID equals account.ID_NUM
                where account.ID_NUM == "50153295"
                select new HomeResidentViewModel{ RoomID = resRCI.RoomRCIID, FirstName = account.firstname, LastName = account.lastname};
            // haven't find a correct way to access room id, 
            // and the database might be changed to connect
            // resRCI directly to room id. so just use roomrciid
            // for now
            return View(resRCIs);
        }

        // GET: Home/RA
        public ActionResult RA()
        {
            // Display all RCI's for the corresponding building

            var resRCIs =
                from resRCI in db.ResidentRCI
                join account in db.Account on resRCI.ResidentAccountID equals account.ID_NUM
                select new HomeResidentViewModel { RoomID = resRCI.RoomRCIID, FirstName = account.firstname, LastName = account.lastname };
            // haven't find a correct way to access room id, 
            // and the database might be changed to connect
            // resRCI directly to room id. so just use roomrciid
            // for now
            return View(resRCIs);
        }

        // GET: Home/RD
        public ActionResult RD()
        {
            // Display all RCI's for the corresponding building

            var resRCIs =
                from resRCI in db.ResidentRCI
                join account in db.Account on resRCI.ResidentAccountID equals account.ID_NUM
                select new HomeResidentViewModel { RoomID = resRCI.RoomRCIID, FirstName = account.firstname, LastName = account.lastname };
            // haven't find a correct way to access room id, 
            // and the database might be changed to connect
            // resRCI directly to room id. so just use roomrciid
            // for now
            return View(resRCIs);
        }

        // Potentially later: admin option that can view all RCI's for all buildings
    }
}