using System;
using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Filters;
using Phoenix.Services;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class HomeController : Controller
    {
        // RCI context wrapper. It can be considered to be an object that represents the database.
        private RCIContext db;
        private DashboardService dashboardService;

        public HomeController()
        {
            db = new Models.RCIContext();
            dashboardService = new DashboardService(db);
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
            var strBuilding = (string)TempData["building"];
            var strRoomNumber = (string)TempData["room"];

            var RCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                where account.ID_NUM == strID && personalRCI.Current == true
                select new HomeRCIViewModel
                {
                    RCIID = personalRCI.RCIID,
                    BuildingCode = personalRCI.BuildingCode,
                    RoomNumber = personalRCI.RoomNumber,
                    FirstName = account.firstname,
                    LastName = account.lastname
                };

            // Check if current RCI corresponds to user's current building and room, as defined in RoomAssign
            //var RCIsForCurrentBuilding = RCIs.Where(m => m.BuildingCode == strBuilding && m.RoomNumber == strRoomNumber);


            RCIs = (IQueryable<HomeRCIViewModel>)dashboardService.ValidateResidentsRCIsExistence(RCIs, strBuilding, strRoomNumber, strID);

            return View(RCIs);
        }

        // GET: Home/RA
        public ActionResult RA()
        {
            // TempData stores object, so always cast to string.
            var strID = (string)TempData["id"];
            var strBuilding = (string)TempData["building"];
            var strRoomNumber = (string)TempData["room"];

            // Query just for the RA's own RCI
            var RCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM
                where account.ID_NUM == strID && personalRCI.Current == true
                select new HomeRCIViewModel
                {
                    RCIID = personalRCI.RCIID,
                    BuildingCode = personalRCI.BuildingCode,
                    RoomNumber = personalRCI.RoomNumber,
                    FirstName = account.firstname,
                    LastName = account.lastname
                };
            // Verify that the RA actually has their own RCIs set up
            dashboardService.ValidateResidentsRCIsExistence(RCIs, strBuilding, strRoomNumber, strID);

            // Display all RCI's for the corresponding building
            // Not sure if this will end up with duplicates for the RA's own RCI
            var buildingRCIs =
                from personalRCI in db.RCI
                join account in db.Account on personalRCI.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where personalRCI.BuildingCode == strBuilding && personalRCI.Current == true
                select new HomeRCIViewModel
                {
                    RCIID = personalRCI.RCIID,
                    BuildingCode = personalRCI.BuildingCode,
                    RoomNumber = personalRCI.RoomNumber,
                    FirstName = account.firstname == null ? "Common Area" : account.firstname,
                    LastName = account.lastname == null ? "RCI" : account.lastname
                };

            RCIs = RCIs.Concat(buildingRCIs);

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
                select new HomeRCIViewModel
                {
                    RCIID = personalRCI.RCIID,
                    BuildingCode = personalRCI.BuildingCode,
                    RoomNumber = personalRCI.RoomNumber,
                    FirstName = account.firstname == null ? "Common Area" : account.firstname,
                    LastName = account.lastname == null ? "RCI" : account.lastname
                };
            
            return View(RCIs);
        }

        // Potentially later: admin option that can view all RCI's for all buildings

        //public int GenerateRCI(string buildingCode, string roomNumber, string id = null )
        //{
        //    var newRCI = new RCI();
        //    newRCI.GordonID = id;
        //    newRCI.BuildingCode = buildingCode;
        //    newRCI.RoomNumber = roomNumber;
        //    newRCI.Current = true;
        //    newRCI.CreationDate = DateTime.Now;

        //    db.RCI.Add(newRCI);
        //    db.SaveChanges();

        //    return newRCI.RCIID;

        //}

//        public void AddRCIComponents(int rciId, string roomType)
//        {
//            var componentNames = new List<string>();
//            if (roomType.Equals("common area"))
//            {
//                componentNames.AddRange(new string[]{ "Carpet", "Couch", "Sink",
//                    "Living room table", "Kitchen table", "Kitchen Chairs"});
//            }
//            else // for now, just generic dorm; will add more checks once we've determined which components go where
//            {
//                componentNames.AddRange(new string[] { "Bed", "Carpet", "Desk", "Desk Chair",
//                    "Dresser", "Wall", "Wardrobe" });
//;           }
//            foreach(var name in componentNames)
//            {
//                var newComponent = new RCIComponent();
//                newComponent.RCIComponentName = name.ToString();
//                newComponent.RCIID = rciId;

//                db.RCIComponent.Add(newComponent);
//                db.SaveChanges();
//            }
//        }
    }
}