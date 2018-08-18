namespace Phoenix.DapperDal.Sql
{
    public class Account
    {
        public const string AccountSelectStatement =
            @"
	select account.ID_NUM as GordonId,
		account.firstname as FirstName,
		account.lastname as LastName,
		account.AD_Username as AdUsername,
		account.email as Email,
		case when cra.ID_NUM is null then 0 else 1 END as IsRa,
		RTRIM(cra.Dorm) as RaBuildingCode,
		case when crd.ID_NUM is null then 0 else 1 END as IsRd,
		crd.Job_Title_Hall as RdHallGroup,
		case when adm.GordonID is null then 0 else 1 END as isAdmin
	from Account account
	left join CurrentRA cra
	on account.ID_NUM = cra.ID_NUM
	left join CurrentRD crd
	on account.ID_NUM = crd.ID_NUM
	left join [Admin] adm
	on account.ID_NUM = adm.GordonID
";

        public const string ResidentAccountsSelectStatement =
            @"select roomAssign.ID_NUM as GordonId,
		account.firstname as FirstName,
		account.lastname as LastName,
		account.AD_Username as AdUsername,
		account.email as Email,
		case when cra.ID_NUM is null then 0 else 1 END as IsRa,
		cra.Dorm as RaBuildingCode,
		case when crd.ID_NUM is null then 0 else 1 END as IsRd,
		crd.Job_Title_Hall as RdHallGroup,
		case when adm.GordonID is null then 0 else 1 END as isAdmin
from RoomAssign roomAssign
left join Account account
on roomAssign.ID_NUM = account.ID_NUM
left join CurrentRA cra
on account.ID_NUM = cra.ID_NUM
left join CurrentRD crd
on account.ID_NUM = crd.ID_NUM
left join [Admin] adm
on account.ID_NUM = adm.GordonID
";
    }
}