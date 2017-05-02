using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Phoenix.Controllers
{
    public class ManageRciComponentController : Controller
    {
        // GET: ManageRciComponent
        public ActionResult Index(string buildingCode, string roomType)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            ViewBag.RCI_TYPE_COMMON = Constants.RCI_TYPE_COMMON;
            ViewBag.RCI_TYPE_INDIVIDUAL = Constants.RCI_TYPE_INDIVIDUAL;
            return View(rci);
        }

        [HttpPost]
        public void EditComponentDescription(string buildingCode, string roomType, string componentName, string componentDescription, string newDescription)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName)
                .Where(x => (string)x.Attribute("description") == componentDescription)
                .FirstOrDefault();
            component.Attribute("description").SetValue(newDescription);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void EditBuildingCode(string buildingCode, string roomType, string newBuildingCode)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            rci.Attribute("buildingCode").SetValue(newBuildingCode);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void EditCost(string buildingCode, string roomType, string componentName, string componentDescription, string oldCostName, string oldCostApproxCost, string newCostName, string newCostApproxCost)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName)
                .Where(x => (string)x.Attribute("description") == componentDescription)
                .FirstOrDefault();
            XElement cost = component.Descendants("cost")
                .Where(x => (string)x.Attribute("name") == oldCostName)
                .Where(x => (string)x.Attribute("approxCost") == oldCostApproxCost)
                .FirstOrDefault();
            cost.Attribute("name").SetValue(newCostName);
            cost.Attribute("approxCost").SetValue(newCostApproxCost);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void AddCost(string buildingCode, string roomType, string componentName, string componentDescription, string newCostName, string newCostApproxCost)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName)
                .Where(x => (string)x.Attribute("description") == componentDescription)
                .FirstOrDefault();
            XElement newCost = new XElement("cost");
            XAttribute costName = new XAttribute("name", newCostName);
            XAttribute costApproxCost = new XAttribute("approxCost", newCostApproxCost);
            newCost.Add(costName);
            newCost.Add(costApproxCost);
            component.Add(newCost);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void DeleteCost(string buildingCode, string roomType, string componentName, string componentDescription, string name, string approxCost)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName)
                .Where(x => (string)x.Attribute("description") == componentDescription)
                .FirstOrDefault();
            XElement cost = component.Descendants("cost")
                .Where(x => (string)x.Attribute("name") == name)
                .Where(x => (string)x.Attribute("approxCost") == approxCost)
                .FirstOrDefault();
            cost.Remove();
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void EditComponentName(string buildingCode, string roomType, string oldComponentName, string oldComponentDescription, string newComponentName)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == oldComponentName)
                .Where(x => (string)x.Attribute("description") == oldComponentDescription)
                .FirstOrDefault();
            component.Attribute("name").SetValue(newComponentName);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void AddComponent(string buildingCode, string roomType, string newComponentName, string newComponentDescription)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement components = rci.Element("components");
            XElement component = new XElement("component");
            XAttribute name = new XAttribute("name", newComponentName);
            XAttribute description = new XAttribute("description", newComponentDescription);
            component.Add(name);
            component.Add(description);
            components.Add(component);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void DeleteComponent(string buildingCode, string roomType, string componentName, string componentDescription)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName)
                .Where(x => (string)x.Attribute("description") == componentDescription)
                .FirstOrDefault();
            component.Remove();
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void SwapComponents(string buildingCode, string roomType, string componentName1, string componentDescription1, string componentName2, string componentDescription2)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            XElement component1 = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName1)
                .Where(x => (string)x.Attribute("description") == componentDescription1)
                .FirstOrDefault();
            XElement component2 = rci.Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName2)
                .Where(x => (string)x.Attribute("description") == componentDescription2)
                .FirstOrDefault();
            component1.ReplaceWith(component2);
            component2.ReplaceWith(component1);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void EditRoomType(string buildingCode, string roomType, string newRoomType)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci")
                .Where(x => (string)x.Attribute("buildingCode") == buildingCode)
                .Where(x => (string)x.Attribute("roomType") == roomType)
                .FirstOrDefault();
            rci.Attribute("roomType").SetValue(newRoomType);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }
    }
}