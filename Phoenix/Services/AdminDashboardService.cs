using Phoenix.Models;
using Phoenix.Models.ViewModels;
using Phoenix.Utilities;
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

    }
}