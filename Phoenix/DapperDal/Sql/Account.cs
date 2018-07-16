namespace Phoenix.DapperDal.Sql
{
    public class Account
    {
        public const string AccountSelectStatement =
            @"select * from 
(
	select a.ID_NUM as GordonId,
		a.firstname as FirstName,
		a.lastname as LastName,
		a.AD_Username as AdUsername,
		a.email as Email,
		case when cra.ID_NUM is null then 0 else 1 END as IsRa,
		cra.Dorm as RaBuildingCode,
		case when crd.ID_NUM is null then 0 else 1 END as IsRd,
		crd.Job_Title_Hall as RdHallGroup,
		case when adm.GordonID is null then 0 else 1 END as isAdmin
	from Account a
	left join CurrentRA cra
	on a.ID_NUM = cra.ID_NUM
	left join CurrentRD crd
	on a.ID_NUM = crd.ID_NUM
	left join [Admin] adm
	on a.ID_NUM = adm.GordonID ) As account
";
    }
}