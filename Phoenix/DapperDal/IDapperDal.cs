using Phoenix.DapperDal.Types;
using System.Collections.Generic;

namespace Phoenix.DapperDal
{
    public interface IDal
    {

        int CreateNewDormRci(string gordonId, string buildingCode, string roomNumber, string sessionCode);
        int CreateNewCommonAreaRci(string buildingCode, string roomNumber, string sessionCode);
        void SetRciIsCurrentColumn(IEnumerable<int> rciIds, bool isCurrent);
        void DeleteRci(int rciId);

        BigRci FetchRciById(int rciId);
        List<SmolRci> FetchRcisByGordonId(string gordonId);
        List<SmolRci> FetchRcisByBuilding(List<string> buildings);
        List<SmolRci> FetchRcisBySessionAndBuilding(List<string> sessions, List<string> buildings);
        List<SmolRci> FetchRcisForRoom(string building, string room);

        List<FineSummary> FetchFinesByBuilding(List<string> buildings);

        List<string> FetchBuildingCodes();
        List<ResidentHallGrouping> FetchBuildingMap();

        Room FetchRoom(string buildingCode, string roomNumber);

        List<RoomComponentType> FetchRoomComponentTypesForRci(int rciId);

        RoomAssignment FetchLatestRoomAssignmentForId(string id);
        List<RoomAssignment> FetchRoomAssignmentsThatDoNotHaveRcis(string buildingCode, string sessionCode);

        List<Session> FetchSessions();
        
        Account FetchAccountByGordonId(string gordonId);
        List<Account> FetchResidentAccounts(string builingCode, string roomNumber, string sessionCode);

    }
}