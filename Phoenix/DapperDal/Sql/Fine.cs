namespace Phoenix.DapperDal.Sql
{
    public class Fine
    {
        public const string FineSelectStatement =
            @"select fine.FineID as FineId,
		fine.FineAmount as Amount,
		fine.GordonID as GordonId,
		fine.Reason as Reason,
		fine.RciId as RciId,
		fine.RoomComponentTypeId as RoomComponentTypeId
from Fine
";

        public const string FineSummarySelectStatement =
            @"select fine.FineID as FineId,
		rci.IsCurrent as IsCurrent,
		rci.BuildingCode as BuildingCode,
		rci.RoomNumber as RoomNumber,
		rci.SessionCode as SessionCode,
		roomComponentType.RoomComponentName as RoomComponentName,
		roomComponentType.RoomArea as RoomArea,
		roomComponentType.SuggestedCosts as SuggestedCostsString,
		fine.FineAmount as FineAmount,
		fine.GordonID as GordonId,
		fine.Reason as Reason,
		acct.firstname as FirstName,
		acct.lastname as LastName,
		acct.email as Email
from Rci rci
inner join Fine fine
on fine.RciId = rci.RciId
inner join Account acct
on fine.GordonId = acct.ID_NUM
inner join RoomComponentType roomComponentType
on fine.RoomComponentTypeId = roomComponentType.RoomComponentTypeId
";
    }
}