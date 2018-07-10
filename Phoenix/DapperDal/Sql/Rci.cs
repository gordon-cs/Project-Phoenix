namespace Phoenix.DapperDal.Sql
{
    public class Rci
    {
        public const string SmolRciSelectstatement =
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
		rci.RoomNumber as RoomNumber,
		rci.SessionCode as SessionCode,
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
		rci.CheckinSigRDGordonID as CheckinRdGordonId
from Rci as rci
left join Account as account
on rci.GordonID = account.ID_NUM
 ";
        public const string BigRciSelectStatement =
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
		rci.RoomNumber as RoomNumber,
		rci.SessionCode as SessionCode,
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
		commonAreaSignature.GordonID as CommonAreaSignatures_GordonId,
		commonAreaSignature.RciID as CommonAreaSignatures_RciId,
		commonAreaSignature.SignatureType as CommonAreaSignatures_SignatureType,
		commonAreaSignature.[Signature] as CommonAreaSignatures_SignatureDate,
		rciComponent.RciComponentID as RciComponents_RciComponentId,
		rciComponent.RciComponentName as RciComponents_RciComponentName,
		rciComponent.RciComponentDescription as RciComponents_RciComponentDescription,
		rciComponent.SuggestedCosts as RciComponents_SuggestedCosts,
		damage.DamageID as RciComponents_Damages_DamageId,
		damage.DamageDescription as RciComponents_Damages_Description,
		damage.DamageImagePath as RciComponents_Damages_ImagePath,
		damage.DamageType as RciComponents_Damages_DamageType,
		fine.FineID as RciComponents_Fines_FineId,
		fine.FineAmount as RciComponents_Fines_Amount,
		fine.GordonID as RciComponents_Fines_GordonId,
		fine.Reason as RciComponents_Fines_Reason,
		fine.RciComponentID as RciComponents_Fines_RciComponentId
from Rci as rci
left join Account as account
on rci.GordonID = account.ID_NUM
left join CommonAreaRciSignature as commonAreaSignature
on commonAreaSignature.RciID = rci.RciID
left join RciComponent as rciComponent
on rciComponent.RciID = rci.RciID
left join Damage as damage
on damage.RciComponentID = rciComponent.RciComponentID
left join Fine as fine
on fine.RciComponentID = rciComponent.RciComponentID
";
    }
}