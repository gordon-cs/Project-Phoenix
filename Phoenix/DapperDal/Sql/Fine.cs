namespace Phoenix.DapperDal.Sql
{
    public class Fine
    {
        public const string FineSummarySelectStatement =
            @"select fine.FineID as FineId,
		rci.IsCurrent as IsCurrent,
		rci.BuildingCode as BuildingCode,
		rci.RoomNumber as RoomNumber,
		rci.SessionCode as SessionCode,
		comp.RciComponentName as RciComponentName,
		comp.RciComponentDescription as RciComponentDescription,
		comp.SuggestedCosts as SuggestedCostsString,
		fine.FineAmount as FineAmount,
		fine.GordonID as GordonId,
		fine.Reason as Reason,
		acct.firstname as FirstName,
		acct.lastname as LastName,
		acct.email as Email
from Rci rci
inner join RciComponent comp
on rci.RciId = comp.RciId
inner join Fine fine
on comp.RciComponentID = fine.RciComponentID
inner join Account acct
on fine.GordonID = acct.ID_NUM
";
    }
}