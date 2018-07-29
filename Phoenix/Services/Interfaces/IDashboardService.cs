using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml.Linq;
using Phoenix.Models;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public interface IDashboardService
    {
        IEnumerable<HomeRciViewModel> GetCurrentRcisForBuilding(List<string> buildingCode);
        IEnumerable<HomeRciViewModel> GetCurrentRcisForResident(string gordonId);
        IEnumerable<HomeRciViewModel> GetCurrentCommonAreaRcisForRoom(string currentRoom, string building);
        void ArchiveRcis(List<int> rciIds);
        void SwapRciOwners(int rci1, int rci2);
        string GenerateFinesSpreadsheet(List<string> buildingCodes);
        string GetCurrentSession();
        Dictionary<string, Dictionary<string, ActionResult>> GetRciRouteDictionary(int rciID);
        string GetRciState(int rciID);
        void SyncCommonAreaRcisFor(string buildingCode, string roomNumber);
        void SyncRoomRcisFor(List<string> kingdom);
        void SyncRoomRcisFor(string buildingCode, string roomNumber, string idNumber, DateTime? roomAssignDate);
    }
}