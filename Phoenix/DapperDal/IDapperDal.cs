using Phoenix.DapperDal.Types;
using System.Collections.Generic;

namespace Phoenix.DapperDal
{
    public interface IDal
    {
       
        BigRci FetchRciById(int rciId);
        List<BigRci> FetchRcisByGordonId(string gordonId);
        List<SmolRci> FetchRcisByBuilding(List<string> buildings);
        List<SmolRci> FetchRcisBySessionAndBuilding(List<string> sessions, List<string> buildings);
        List<SmolRci> FetchRcisForRoom(string building, string room);

        List<FineSummary> FetchFinesByBuilding(List<string> buildings);

        List<string> FetchBuildingCodes();
        List<ResidentHallGrouping> FetchBuildingMap();

        RoomAssignment FetchLatestRoomAssignmentForId(string id);

        List<Session> FetchSessions();
        
        Account FetchAccountByGordonId(string gordonId);

    }
}