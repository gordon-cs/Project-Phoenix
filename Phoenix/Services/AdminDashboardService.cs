using Phoenix.DataAccessLayer;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var rciQueries = new RciQueries();

            var results = from rci in rciQueries.Rcis()
                            where sessions.Contains(rci.SessionCode) && buildings.Contains(rci.BuildingCode)
                            &&( (rci.GordonID != null && keyword.Contains(rci.GordonID)) 
                            || keyword.Contains(rci.BuildingCode)
                            || keyword.Contains(rci.SessionCode) 
                            || keyword.Contains(rci.RoomNumber)
                            || (rci.FirstName + " " + rci.LastName).Contains(keyword))
                            select new HomeRciViewModel
                            {
                                RciID = rci.RciID,
                                BuildingCode = rci.BuildingCode.Trim(),
                                RoomNumber = rci.RoomNumber.Trim(),
                                FirstName = rci.FirstName,
                                LastName = rci.LastName,
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
        // Returns a tuple of strings representing each type of RCI, 
        //where Item 1 is the building code and Item 2 is the room type (either common (area) or individual)
        public IEnumerable<Tuple<string, string>> GetRciTypes(XDocument document)
        {
            IEnumerable<XElement> rciTypes = document.Root.Elements("rci");

            List<Tuple<string, string>> result = new List<Tuple<string, string>>();

            foreach (var e in rciTypes)
            {
                var buildingCode = e.Attribute("buildingCode").Value;
                string dormStyle;
                
                if (e.Attribute("roomType").Value.Equals("common"))
                {
                    dormStyle = "common";
                }
                else
                {
                    dormStyle = "individual";
                }

                result.Add(new Tuple<string, string>(buildingCode, dormStyle));

            }

            return result;
                       
           
    }

    }
}