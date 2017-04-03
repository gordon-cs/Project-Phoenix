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
            viewModel.Buildings = adminDashboardService.GetBuildingCodes();
            viewModel.Sessions = adminDashboardService.GetSessions();

            return View(viewModel);
        }
    }
}