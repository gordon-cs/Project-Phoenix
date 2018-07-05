using Dapper;
using Phoenix.DapperDal.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Phoenix.DapperDal
{
    public class DapperDal : IDal
    {
        public readonly IDbConnectionFactory _dbConnectionFactory;

        public DapperDal(IDbConnectionFactory factory)
        {
            this._dbConnectionFactory = factory;
        }

        public List<ResidentHallGrouping> FetchBuildingMap()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = @"select JobTitleHall as HallGroup, BuildingCode as BuildingCodes_$ from BuildingAssign";

                var queryResult = connection.Query(sql).ToList();

                var mapperResult = Slapper.AutoMapper.MapDynamic<ResidentHallGrouping>(queryResult).ToList();

                return mapperResult;
            }
        }

        public List<string> FetchBuildingCodes()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = @"select BuildingCode from BuildingAssign group by BuildingCode";

                return connection.Query<string>(sql).ToList();
            }
        }

        public List<Session> FetchSessions()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = @"select s.SESS_CDE as SessionCode, 
                            s.SESS_DESC as SessionDescription, 
                            s.SESS_BEGN_DTE as SessionStartDate, 
                            s.SESS_END_DTE as SessionEndDate
                            from session as s";

                return connection.Query<Session>(sql).ToList();
            }
        }

        public BigRci FetchRciById(int rciId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = BigRciSelectStatement + "where rci.RciId = @RciId";

                var queryResult = connection.Query(sql, new { RciId = rciId });

                var mapperResult = Slapper.AutoMapper.MapDynamic<BigRci>(queryResult);

                BigRci retVal;

                try
                {
                    retVal = mapperResult.Single();

                    retVal.PopulateFirstAndLastName();
                }
                catch (InvalidOperationException e)
                {
                    var errorMessage = $"Expected a single result to be returned for RciId {rciId}. Got {mapperResult.Count()}";

                    throw new Exception(errorMessage, e);
                }

                return retVal;
            }
        }

        public List<BigRci> FetchRciByGordonId(string gordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = BigRciSelectStatement + "where rci.GordonId = @GordonId";

                var queryResult = connection.Query(sql, new { GordonId = gordonId });

                var mapperResult = Slapper.AutoMapper.MapDynamic<BigRci>(queryResult).ToList();

                mapperResult.ForEach(x => x.PopulateFirstAndLastName());

                return mapperResult.ToList();
            }
        }

        public List<SmolRci> FetchRciBySessionAndBuilding(List<string> sessions, List<string> buildings)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = SmolRciSelectstatement +
                    "where rci.SessionCode in @SessionCodes and rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query(sql, new { SessionCodes = sessions, BuildingCodes = buildings });

                var mapperResult = Slapper.AutoMapper.MapDynamic<SmolRci>(queryResult).ToList();

                mapperResult.ForEach(x => x.PopulateFirstAndLastName());

                return mapperResult;
            }
        }

        public Account FetchAccountByGordonId(string gordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = AccountSelectStatement +
                    "where account.GordonId = @Id";

                var queryResult = connection.Query<Account>(sql, new { Id = gordonId });

                Account account = null;

                try
                {
                    account = queryResult.Single();
                }
                catch (InvalidOperationException e)
                {
                    throw new Exception($"Excpected exactly one account result, got {queryResult.Count()}", e);
                }

                return account;
            }
        }


        public RoomAssignment FetchLatestRoomAssignmentForId(string id)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = RoomAssignmentSelectStatment +
                    "where ID_NUM = @Id order by ASSIGN_DTE desc";

                var queryResult = connection.Query<RoomAssignment>(sql, new { Id = id }).ToList();

                var latestRoomAssign = queryResult.FirstOrDefault();

                // For now, we are ok with returning nulls
                return latestRoomAssign;
            }
        }
        // Don't forget the newline at the end of the SQL string:)

        private const string SmolRciSelectstatement =
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
        private const string BigRciSelectStatement =
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

        private const string AccountSelectStatement =
            @"select * from 
(
	select a.ID_NUM as GordonId,
		a.firstname as FirstName,
		a.lastname as LastName,
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

        private const string RoomAssignmentSelectStatment =
            @"select ID_NUM as GordonId,
		BLDG_CDE as BuildingCode,
		ROOM_CDE as RoomNumber,
		ROOM_TYPE as RoomType,
		SESS_CDE as SessionCode,
		ASSIGN_DTE as AssignmentDate
from RoomAssign
";


    }
}