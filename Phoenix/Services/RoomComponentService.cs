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

        /// <summary>
        /// Compile a dictionary of rci components and associated costs. The list of components is retrieved from
        /// the RoomComponents.xml file.
        /// If two components have the same name and have different costs in the xml file, the costs are joined.
        /// E.g - In a common area, the two Wall components will display the same cost string, which will be the the collection 
        /// of the individual costs indicated in the xml. A HashSet is used to remove duplicates.
        /// </summary>
        public Dictionary<string, HashSet<string>> GetCostDictionary(string roomType, string buildingCode)
        {
            var costDictionary = new Dictionary<string, HashSet<string>>();

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
                if(costDictionary.ContainsKey(componentName))
                {
                    foreach(var cost in costs)
                    {
                        costDictionary[componentName].Add(cost);
                    }
                }
                else
                {
                    costDictionary.Add(componentName, new HashSet<string>(costs));
                }

            }

            return costDictionary;

        }
    }
}