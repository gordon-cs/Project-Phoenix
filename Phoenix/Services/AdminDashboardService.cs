using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                           where fourYearsAgo.CompareTo(entry.SESS_BEGN_DTE) >= 0
                           select entry;

            // Convert query result to a dictionary of <key=Session Code, value=Session Description>
            IDictionary<string, string> sessionDictionary = sessions.ToDictionary(s => s.SESS_CDE, s => s.SESS_DESC);

            return sessionDictionary;
        }

    }
}