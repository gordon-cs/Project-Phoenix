using Phoenix.Filters;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Phoenix.Services;

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
    }
}