namespace Phoenix.DapperDal.Sql
{
    public class Rci
    {
        public const string RciInsertStatement =
            @"insert into Rci(IsCurrent,CreationDate,BuildingCode,RoomNumber,GordonID,SessionCode,RciTypeId,CheckinSigRes,CheckinSigRA,CheckinSigRD,LifeAndConductSigRes,CheckoutSigRes,CheckoutSigRA,CheckoutSigRD,CheckoutSigRAGordonID,CheckoutSigRDGordonID,CheckinSigRAGordonID,CheckinSigRDGordonID)
values (@IsCurrent,@CreationDate,@BuildingCode, @RoomNumber, @GordonId,@SessionCode,@RciTypeId, null, null, null, null, null, null, null, null, null, null, null)
SELECT CAST(SCOPE_IDENTITY() as int)
";

        public const string RciSelectstatement =
            @"select rci.RciId as RciId,
		rci.GordonID as GordonId,
		account.firstname as FirstName,
		account.lastname as LastName,
		account.email as Email,
		account.AD_Username as AdUsername,
		account.account_type as AccountType,
		rci.IsCurrent as IsCurrent,
		rci.CreationDate as CreationDate,
		rci.BuildingCode as BuildingCode,
		building.BuildingDescription as BuildingDescription,
		rci.RoomNumber as RoomNumber,
		rci.SessionCode as SessionCode,
		sess.SESS_DESC as SessionDescription,
		rci.CheckinSigRes as ResidentCheckinDate,
		rci.CheckinSigRA as RaCheckinDate,
		rci.CheckinSigRD as RdCheckinDate,
		rci.LifeAndConductSigRes as LifeAndConductSignatureDate,
		rci.CheckoutSigRes as ResidentCheckoutDate,
		rci.CheckoutSigRA as RaCheckoutDate,
		rci.CheckoutSigRD as RdCheckoutDate,
		rci.CheckoutSigRAGordonID as CheckoutRaGordonId,
		rci.CheckoutSigRDGordonID as CheckoutRdGordonId,
		rci.CheckinSigRAGordonID as CheckinRaGordonId,
		rci.CheckinSigRDGordonID as CheckinRdGordonId,
		rci.RciTypeId as RciTypeId,
		rciType.RciTypeName as RciTypeName
from Rci as rci
left join Account as account
on rci.GordonID = account.ID_NUM
inner join RciType rciType
on rci.RciTypeId = rciType.RciTypeId
inner join [Session] sess
on rci.SessionCode = sess.SESS_CDE
inner join (select BLDG_CDE as BuildingCode, BUILDING_DESC as BuildingDescription from Room group by BLDG_CDE, BUILDING_DESC) as building
on rci.BuildingCode = building.BuildingCode
 ";
    }
}