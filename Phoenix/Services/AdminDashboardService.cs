using Dapper;
using Newtonsoft.Json;
using Phoenix.DapperDal;
using Phoenix.DapperDal.Types;
using Phoenix.DataAccessLayer;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Phoenix.Services
{
    public class AdminDashboardService
    {
        private readonly IDapperDal Dal;

        public AdminDashboardService()
        {
            this.Dal = new DapperDal.DapperDal();
        }

        public AdminDashboardService(IDapperDal _dal)
        {
            this.Dal = _dal;
        }
        
        public IEnumerable<string> GetBuildingCodes()
        {
            return this.Dal.FetchBuildingCodes();
        }

        /* Get a list of session codes for the last 4 years
         */
        public IDictionary<string, string> GetSessions()
        {
            DateTime fourYearsAgo = DateTime.Today.AddYears(-4);

            var sessions = this.Dal.FetchSessions();

            // now filter out only recent sessions
            sessions = sessions
                .Where(x => fourYearsAgo.CompareTo(x.SessionStartDate.Value) <= 0)
                .OrderByDescending(m => m.SessionStartDate)
                .ToList();

            // Convert query result to a dictionary of <key=Session Code, value=Session Description>
            IDictionary<string, string> sessionDictionary = sessions.ToDictionary(s => s.SessionCode.Trim(), s => s.SessionDescription.Trim());

            return sessionDictionary;
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

        public List<HomeRciViewModel> Search(IEnumerable<string> sessions, IEnumerable<string> buildings, string keyword)
        {
            // Filter all the rcis by the session and building codes provided
            var filteredRcis = this.Dal.FetchRciBySessionAndBuilding(sessions.ToList(), buildings.ToList());

            List<SmolRci> searchResults = filteredRcis;

            // Of the returned rcis, search for the keyword inside specific fields
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                searchResults = filteredRcis
                    .Where(x =>
                    {
                        if (x.FirstName != null && x.FirstName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;
                        if (x.LastName != null && x.LastName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;
                        if (x.FirstName != null && x.LastName != null && $"{x.FirstName} {x.LastName}".IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;
                        if (x.GordonId != null && x.GordonId.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;
                        if (x.RoomNumber != null && x.RoomNumber.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;
                        return false;
                    })
                    .ToList();
            }

            return searchResults
                .Select(x =>
                {
                    return new HomeRciViewModel
                    {
                        RciID = x.RciId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        BuildingCode = x.BuildingCode,
                        RoomNumber = x.RoomNumber,
                        CheckinSigRes = x.ResidentCheckinDate,
                        CheckinSigRA = x.RaCheckinDate,
                        CheckinSigRD = x.RdCheckinDate,
                        CheckoutSigRes = x.ResidentCheckoutDate,
                        CheckoutSigRA = x.RaCheckoutDate,
                        CheckoutSigRD = x.RdCheckoutDate,
                        RciStage = x.RaCheckinDate == null ? Constants.RCI_CHECKIN_STAGE : Constants.RCI_CHECKOUT_STAGE
                    };
                })
                .ToList();

        }
    }
}