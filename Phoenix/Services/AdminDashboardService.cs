using Phoenix.DapperDal;
using Phoenix.DapperDal.Types;
using Phoenix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Phoenix.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IDatabaseDal Dal;

        public AdminDashboardService(IDatabaseDal _dal)
        {
            this.Dal = _dal;
        }
        
        public Dictionary<string, string> GetBuildingCodes()
        {
            return this.Dal.FetchBuildingCodeToBuildingNameMap();
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

        public List<HomeRciViewModel> Search(IEnumerable<string> sessions, IEnumerable<string> buildings, string keyword)
        {
            // Filter all the rcis by the session and building codes provided
            var filteredRcis = this.Dal.FetchRcisBySessionAndBuilding(sessions.ToList(), buildings.ToList());

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
                .Select(x => new HomeRciViewModel(x))
                .ToList();

        }
    }
}