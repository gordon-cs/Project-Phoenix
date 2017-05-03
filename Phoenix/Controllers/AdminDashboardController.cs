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

        /* POST: Create a new type of RCI
          This will involve adding a new <rciType> to the XML
          If a prexisting RCI has been selected to copy from, copy from that, else just create empty XML element for <components>
          Once the new type has been created, we want to redirect user to this new page
          Params: buildingCode: the code for the building this RCI will be used for (e.g. WIL)
                  roomType: the type of dorm room (either "common" or "individual")
                  copyOption: the prexisting RCI type that the user wants to copy over to create a new one. Could be "none"
                              if user wants to start from scratch
        */          
        [HttpPost]
        public JsonResult AddNewRciType(string buildingCode, string roomType, string copyOption)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;

            XElement newType;

            if (copyOption.Equals("None"))
            {
                // If the user did not select a pre-existing RCI to copy from, then create a new one
                newType = new XElement("rci");

                XAttribute buildingCodeAttribute = new XAttribute("buildingCode", buildingCode);
                newType.Add(buildingCodeAttribute);

                XAttribute roomTypeAttribute = new XAttribute("roomType", roomType);
                newType.Add(roomTypeAttribute);

                XElement componentsHolder = new XElement("components");
                newType.Add(componentsHolder);
            }
            else
            {
                // Find the prexisting rci type for the selected building, and then make a copy of it to modify and save as our new type
                XElement toCopy = rciTypes.Elements("rci")
                            .Where(x => (string)x.Attribute("buildingCode") == copyOption && (string)x.Attribute("roomType") == roomType)
                            .FirstOrDefault();

                if (toCopy == null)
                {
                    throw new ArgumentException("Copy not found");
                }

                newType = new XElement(toCopy);

                // set attributes for new rci type accordingly
                newType.Attribute("buildingCode").SetValue(buildingCode);

                newType.Attribute("roomType").SetValue(roomType);
            }

            rciTypes.Add(newType);

            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));


           // return RedirectToAction(actionName: "Index", controllerName: "ManageRciComponent", routeValues: new { buildingCode = buildingCode, roomType = roomType });

           return Json(Url.Action("Index", "ManageRciComponent", routeValues: new { buildingCode = buildingCode, roomType = roomType }));
        }

        /* POST: Delete a certain rci type (i.e. building)L
           Params: buildingCode: the code for the building this RCI was used for (e.g.WIL)
                  roomType: the type of dorm room(either "common" or "individual")
        */  
        
        public ActionResult DeleteRciType(string buildingCode, string roomType)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement toRemove = document.Root.Elements("rci").Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();

            if (toRemove == null)
            {
                //throw new ArgumentException("Cannot find rci type where building code = " + buildingCode + " and room type = " + roomType);
                return new HttpStatusCodeResult(500, "Cannot find rci type where building code = " + buildingCode + " and room type = " + roomType);
            }

            toRemove.Remove();

            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));

            return new HttpStatusCodeResult(200);
        } 

    }
}