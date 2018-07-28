using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public interface IAdminDashboardService
    {
        IEnumerable<string> GetBuildingCodes();
        IDictionary<string, string> GetSessions();
        List<HomeRciViewModel> Search(IEnumerable<string> sessions, IEnumerable<string> buildings, string keyword);
    }
}