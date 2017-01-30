using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using System.Diagnostics;

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
            // TempData stores object, so always cast to string.
            var role = (string)TempData["role"];


            if (role.Equals("RD"))
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

            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];

            var RCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                where account.ID_NUM == strID && personalRCI.Current == true
                select new HomeRCIViewModel { RCIID = personalRCI.RCIID, BuildingCode = personalRCI.BuildingCode, RoomNumber = personalRCI.RoomNumber, FirstName = account.firstname, LastName = account.lastname };

            var buildingCode = RCIs.FirstOrDefault().BuildingCode.ToString();
            var roomNumber = RCIs.FirstOrDefault().RoomNumber.ToString();

            if (buildingCode.Equals("BRO") || buildingCode.Equals("TAV")) // We have not yet accounted for FERRIN!
            {
                var commonAreaRCIs =
                    from tempCommonAreaRCI in db.RCI
                    where tempCommonAreaRCI.RoomNumber == roomNumber && tempCommonAreaRCI.BuildingCode == buildingCode
                    && tempCommonAreaRCI.GordonID == null
                    select new HomeRCIViewModel
                    {
                        RCIID = tempCommonAreaRCI.RCIID,
                        BuildingCode = tempCommonAreaRCI.BuildingCode,
                        RoomNumber = tempCommonAreaRCI.RoomNumber,
                        FirstName = "Common",
                        LastName = "RCI"
                    };

                RCIs = RCIs.Concat(commonAreaRCIs);
            }

                return View(RCIs);
        }

        // GET: Home/RA
        public ActionResult RA()
        {
            // Display all RCI's for the corresponding building

            // TempData stores object, so always cast to string.
            var strBuilding = (string)TempData["building"];

            var RCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where personalRCI.BuildingCode == strBuilding
                && personalRCI.Current == true
                select new HomeRCIViewModel { RCIID = personalRCI.RCIID, BuildingCode = personalRCI.BuildingCode, RoomNumber = personalRCI.RoomNumber, FirstName = account.firstname == null ? "Common Area" : account.firstname, LastName = account.lastname == null ? "RCI" : account.lastname };

            return View(RCIs);
        }

        // GET: Home/RD
        public ActionResult RD()
        {
            // Display all RCI's for the corresponding building

            // TempData stores object, so always cast to string.
            var strBuilding = (string)TempData["building"];

            string[] strBuildings = (string [])db.BuildingAssign.Where(b => b.Job_Title_Hall.Equals(strBuilding)).Select(b => b.BuildingCode).ToArray();

            var RCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where strBuildings.Contains(personalRCI.BuildingCode) 
                && personalRCI.Current == true
                select new HomeRCIViewModel { RCIID = personalRCI.RCIID, BuildingCode = personalRCI.BuildingCode, RoomNumber = personalRCI.RoomNumber, FirstName = account.firstname == null ? "Common Area" : account.firstname, LastName = account.lastname == null ? "RCI" : account.lastname};
            
            return View(RCIs);
        }

        // Potentially later: admin option that can view all RCI's for all buildings
    }
}