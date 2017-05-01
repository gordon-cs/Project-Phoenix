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
        public ActionResult Index(int id)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement selectedRci =
                rciTypes.Elements("rci").Select((rci, index) =>
                    new XElement("rci",
                        new XAttribute("id", index),
                        rci.Attributes(),
                        rci.Elements()))
                    .ElementAt(id);
            ViewBag.RciID = id;
            return View(selectedRci);
        }

        [HttpPost]
        public void EditComponentDescription(int componentID, int rciID, string newDescription)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component").ElementAt(componentID);
            component.Attribute("description").SetValue(newDescription);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void SaveAddBuilding(int rciID, string newBuilding)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            XAttribute newAttribute = new XAttribute(newBuilding.ToUpper(), "true");
            rci.Add(newAttribute);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void DeleteBuilding(int rciID, string building)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            rci.Attribute(building).Remove();
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void EditCost(int componentID, int rciID, string oldCostName, string oldCostApproxCost, string newCostName, string newCostApproxCost)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component").ElementAt(componentID);
            XElement cost = component.Descendants("cost")
                .Where(x => (string)x.Attribute("name") == oldCostName)
                .Where(x => (string)x.Attribute("approxCost") == oldCostApproxCost)
                .FirstOrDefault();
            cost.Attribute("name").SetValue(newCostName);
            cost.Attribute("approxCost").SetValue(newCostApproxCost);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void AddCost(int componentID, int rciID, string newCostName, string newCostApproxCost)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component").ElementAt(componentID);
            XElement newCost = new XElement("cost");
            XAttribute costName = new XAttribute("name", newCostName);
            XAttribute costApproxCost = new XAttribute("approxCost", newCostApproxCost);
            newCost.Add(costName);
            newCost.Add(costApproxCost);
            component.Add(newCost);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void DeleteCost(int componentID, int rciID, string name, string approxCost)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component").ElementAt(componentID);
            XElement cost = component.Descendants("cost")
                .Where(x => (string)x.Attribute("name") == name)
                .Where(x => (string)x.Attribute("approxCost") == approxCost)
                .FirstOrDefault();
            cost.Remove();
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void EditComponentName(int rciID, string oldComponentName, string oldComponentDescription, string newComponentName)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == oldComponentName)
                .Where(x => (string)x.Attribute("description") == oldComponentDescription)
                .FirstOrDefault();
            component.Attribute("name").SetValue(newComponentName);
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        [HttpPost]
        public void AddComponent(int rciID, string newComponentName, string newComponentDescription)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
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
        public void DeleteComponent(int rciID, string componentName, string componentDescription)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement rci = rciTypes.Elements("rci").ElementAt(rciID);
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component")
                .Where(x => (string)x.Attribute("name") == componentName)
                .Where(x => (string)x.Attribute("description") == componentDescription)
                .FirstOrDefault();
            component.Remove();
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
        }
    }
}