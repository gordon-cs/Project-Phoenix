using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Phoenix.Services
{
    public class AdminDashboardService
    {
        private RCIContext db;

        public AdminDashboardService()
        {
            db = new Models.RCIContext();
        }
        
        /* Get a list of all current building codes in the system
         */
        public IEnumerable<string> GetBuildingCodes()
        {
            var buildingCodes = from entry in db.BuildingAssign
                                select entry.BuildingCode;
            return buildingCodes;
        }


        /* Get a list of session codes for the last 4 years
         */
        public IDictionary<string, string> GetSessions()
        {
            DateTime fourYearsAgo = DateTime.Today.AddYears(-4);

            var sessions = from entry in db.Session
                           select entry;
            // now filter out only recent sessions
            sessions = sessions.Where(x => fourYearsAgo.CompareTo(x.SESS_BEGN_DTE.Value) <= 0).OrderByDescending(m => m.SESS_BEGN_DTE);

            // Convert query result to a dictionary of <key=Session Code, value=Session Description>
            IDictionary<string, string> sessionDictionary = sessions.ToDictionary(s => s.SESS_CDE.Trim(), s => s.SESS_DESC.Trim());

            return sessionDictionary;
        }

        /* Search the RCI db for RCI's that match specified search criteria 
         */
        public IEnumerable<HomeRciViewModel> Search(IEnumerable<string> sessions, IEnumerable<string> buildings, string keyword)
        {
            // Note: These queries are not quite right yet. Maybe I can try refining them in SQL server
           
                var results = from rci in db.Rci
                              join account in db.Account on rci.GordonID equals account.ID_NUM 
                              join sess in db.Session on rci.SessionCode equals sess.SESS_CDE
                              where sessions.Contains(sess.SESS_CDE) && buildings.Contains(rci.BuildingCode)
                              &&( keyword.Contains(rci.GordonID) || keyword.Contains(rci.BuildingCode)
                              || keyword.Contains(rci.SessionCode) || keyword.Contains(rci.RoomNumber)
                              || (account.firstname + " " + account.lastname).Contains(keyword))
                              select new HomeRciViewModel
                              {
                                  RciID = rci.RciID,
                                  BuildingCode = rci.BuildingCode.Trim(),
                                  RoomNumber = rci.RoomNumber.Trim(),
                                  FirstName = account.firstname == null ? "Common Area" : account.firstname,
                                  LastName = account.lastname == null ? "RCI" : account.lastname,
                                  RciStage = rci.CheckinSigRD == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE,
                                  CheckinSigRes = rci.CheckinSigRes,
                                  CheckinSigRA = rci.CheckinSigRA,
                                  CheckinSigRD = rci.CheckinSigRD,
                                  CheckoutSigRes = rci.CheckoutSigRes,
                                  CheckoutSigRA = rci.CheckoutSigRA,
                                  CheckoutSigRD = rci.CheckoutSigRD
                              };

                return results;
 
        }

        // Load all the different types of RCIs from the RoomComponents.xml doc 
        public IEnumerable<string> GetRciTypes(XDocument document)
        {
            IEnumerable<XElement> rciTypes = document.Root.Elements("rci");

            List<string> result = new List<string>();

            foreach (var e in rciTypes)
            {
                var buildings = e.Attributes().Select(x => x.Name).Where(x => (x != "roomType" && x != "id"));

                // If there are multiple buildings accounted for by a certain element <rciType>, we have to give it some overarching label
                // e.g. HUD
                if (buildings.Count()  > 1 )
                {
                    // Check if HUD - originally I was not going to check all options, because if the rciType has one HUD, it has them all
                    // But I thought that might be risky in case one HUD gets removed. (BECAUSE LEWIS... you're days are numbered >.<)
                    if (buildings.Contains("WIL") || buildings.Contains("LEW") || buildings.Contains("EVA")) {
                        result.Add("HUD");
                    }
                    // Check if Road halls
                    else if (buildings.Contains("DEX") || buildings.Contains("GED") || buildings.Contains("GRA") || buildings.Contains("CON")
                        || buildings.Contains("HIL") || buildings.Contains("MCI") || buildings.Contains("RID"))
                    {
                        result.Add("Road Halls");
                    }
                    // Check for Ferrin/Drew
                    else if (buildings.Contains("FER") || buildings.Contains("DRE"))
                    {
                        result.Add("Ferrin/Drew");
                    }
                    else if (buildings.Contains("CHA") || buildings.Contains("FUL"))
                    {
                        result.Add("Chase/Fulton");
                    }
                }

                // Needs thought: all those hard-coded checks above ^^ seem like they will make the system very brittle
                // In order to change which buildings are associated, it will require a source code change. And it will not
                // be possible for a new rci type to be added for multiple buildings in the future.
                // What I am thinking: Maybe we should make it a one-for-one relationship between buildings and RCI types
                // Yes, it will be some duplication, but it allows the user more power/customization when we are gone.
                // #maintainability
                
                else if (e.Attribute("roomType").Value.Equals("common"))
                {
                    result.Add(buildings.First().ToString() + " - Common Area");
                }
                else
                {
                    result.Add(buildings.First().ToString());
                }

            }

            return result;
                       
           
    }

    }
}