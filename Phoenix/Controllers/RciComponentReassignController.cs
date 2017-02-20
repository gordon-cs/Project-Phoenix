using Phoenix.Filters;
using Phoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    public class RciComponentReassignController : Controller
    {

        private RciComponentReassignService reassignService;

        public RciComponentReassignController()
        {
            reassignService = new RciComponentReassignService();
        }

        // GET: RciComponentReassign
        [HttpGet]
        public ActionResult Index(int id)
        {
            var rci = reassignService.GetRciByID(id);
            return View(rci);
        }

        [HttpPost]
        public ActionResult Index(int id, int[] rciComponent, int assignTo )
        {
            reassignService.SwapRciComponents(rciComponent, assignTo, id);
            return RedirectToAction(actionName:"Index", controllerName:"Dashboard");
        }
    }
}