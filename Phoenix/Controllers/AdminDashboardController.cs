using Phoenix.Filters;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Phoenix.Services;
using System.Xml.Linq;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    [Admin]
    public class AdminDashboardController : Controller
    {
        private AdminDashboardService adminDashboardService;

        public AdminDashboardController()
        {
            adminDashboardService = new AdminDashboardService();
        }

        // GET: AdminDashboard
        public ActionResult Index()
        {
            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            SearchResultsViewModel searchViewModel = new SearchResultsViewModel();
            viewModel.Buildings = adminDashboardService.GetBuildingCodes();
            viewModel.Sessions = adminDashboardService.GetSessions();
            viewModel.SearchResults = searchViewModel;

            // Load the RCI types from the XML
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            viewModel.RciTypes = adminDashboardService.GetRciTypes(document);

            return View(viewModel);
        }

        // GET: Search
        public PartialViewResult SearchRcis(IEnumerable<string> sessions, IEnumerable<string> buildings, string keyword)
        {
            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            viewModel.RciSearchResult = adminDashboardService.Search(sessions, buildings, keyword);

            // Note js still needs to add this partial bit of html to the DOM
            return PartialView(viewModel);
        }

        // POST: Create a new type of RCI
        // This will involve adding a new <rciType> to the XML
        // If a prexisting RCI has been selected to copy from, copy from that, else just create empty XML element for <components>
        // Once the new type has been created, we want to redirect user to this new page
        public ActionResult AddNewRciType(string buildingCode, string roomType, string copyOption = null)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement newType = new XElement("rciType");

            XAttribute buildingCodeAttribute = new XAttribute("buildingCode", buildingCode);
            newType.Add(buildingCodeAttribute);

            XAttribute roomTypeAttribute = new XAttribute("roomType", roomType);
            newType.Add(roomTypeAttribute);

            rciTypes.Add(newType);

            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));


            return RedirectToAction("Index", "ManageRciComponents", new { buildingCode = buildingCode, roomType = roomType });
        }
    }
}