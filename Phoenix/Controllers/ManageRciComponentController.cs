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
        public void DeleteComponent(int componentID, int rciID)
        {
            XDocument document = XDocument.Load(Server.MapPath("~/App_Data/RoomComponents.xml"));
            XElement rciTypes = document.Root;
            XElement component = rciTypes.Elements("rci").ElementAt(rciID).Element("components").Elements("component").ElementAt(componentID);
            component.Remove();
            document.Save(Server.MapPath("~/App_Data/RoomComponents.xml"));
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
    }
}