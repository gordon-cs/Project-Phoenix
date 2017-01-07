using System.Linq;
using System.Web.Mvc;

using Phoenix.Models;

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

        // GET: Home/MyRCIs
        public ActionResult MyRCIs()
        {
            var resRCI = db.ResidentRCI.Where(s => s.ResidentAccountID.Equals("50153295"));

            return View(resRCI);
        }
    }
}