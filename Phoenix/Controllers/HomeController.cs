using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;
using Phoenix.Models.ViewModels;

namespace Phoenix.Controllers
{
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
            return View();
        }

        // GET: Home/Resident
        public ActionResult Resident()
        {
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
    }
}