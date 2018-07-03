using Phoenix.DapperDal.Types;
using System.Collections.Generic;

namespace Phoenix.DapperDal
{
    public interface IDal
    {
       
        BigRci FetchRciById(int rciId);
        List<BigRci> FetchRciByGordonId(string gordonId);
        List<SmolRci> FetchRciBySessionAndBuilding(List<string> sessions, List<string> buildings);

        List<string> FetchBuildingCodes();
        List<Session> FetchSessions();
    }
}