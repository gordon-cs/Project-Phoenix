using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Phoenix.Services
{
    public class RoomComponentService
    {
        private XDocument document;
        public RoomComponentService()
        {
            document = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/RoomComponents.xml"));
        }

        public Dictionary<string, List<string>> GetCostDictionary(string roomType, string buildingCode)
        {
            var costDictionary = new Dictionary<string, List<string>>();

            // Get the correct rci template
            var rciTemplate =
                (from rci in document.Root.Elements("rci")
                where ((string)rci.Attribute("roomType")).Equals(roomType) && rci.Attribute(buildingCode) != null
                select rci).FirstOrDefault();

            // Get the components
            var components = rciTemplate.Element("components").Elements("component");

            foreach(var component in components)
            {
                var costs = component.Elements("cost").Select(s => s.Attribute("name").Value + " -  $" + s.Attribute("approxCost").Value).ToList();
                var componentName = component.Attribute("name").Value;
                costDictionary.Add(componentName, costs);

            }

            return costDictionary;

        }
    }
}