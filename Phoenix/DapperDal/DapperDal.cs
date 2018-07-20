using Dapper;
using Phoenix.DapperDal.Types;
using Phoenix.Utilities;
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

        public int CreateNewDormRci(string gordonId, string buildingCode, string roomNumber, string sessionCode)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var insertSql = Sql.Rci.RciInsertStatement;
                var inputParams = new
                {
                    IsCurrent = true,
                    CreationDate = DateTime.Now,
                    BuildingCode = buildingCode,
                    RoomNumber = roomNumber,
                    GordonId = gordonId,
                    SessionCode = sessionCode,
                    RciTypeId = 1
                };

                var insertedId = connection.Query<int>(insertSql, inputParams).Single();

                return insertedId;
            }
        }

        public int CreateNewCommonAreaRci(string buildingCode, string roomNumber, string sessionCode)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var insertSql = Sql.Rci.RciInsertStatement;
                var inputParams = new
                {
                    IsCurrent = true,
                    CreationDate = DateTime.Now,
                    BuildingCode = buildingCode,
                    RoomNumber = roomNumber,
                    GordonId = (int?)null,
                    SessionCode = sessionCode,
                    RciTypeId = 2
                };

                var insertedId = connection.Query<int>(insertSql, inputParams).Single();

                return insertedId;
            }
        }

        public void SetRciIsCurrentColumn(IEnumerable<int> rciIds, bool isCurrent)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = "update Rci set IsCurrent = @IsCurrent where RciId in @RciIds";

                connection.Execute(updateSql, new { IsCurrent = isCurrent, RciIds = rciIds });
            }
        }

        public void SetRciCheckinDateColumns(IEnumerable<int> rciIds, DateTime? residentCheckinDate, DateTime? raCheckinDate, DateTime? rdCheckinDate, DateTime? lifeAndConductStatementCheckinDate)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = new StringBuilder();

                if (residentCheckinDate != null) { updateSql.AppendLine($"update Rci set CheckinSigRes = @ResidentCheckinDate where RciId in @RciIds"); }
                if (raCheckinDate != null) { updateSql.AppendLine($"update Rci set CheckinSigRA = @RaCheckinDate where RciId in @RciIds"); }
                if (rdCheckinDate != null) { updateSql.AppendLine($"update Rci set CheckinSigRD = @RdCheckinDate where RciId in @RciIds"); }
                if (lifeAndConductStatementCheckinDate != null) { updateSql.AppendLine($"update Rci set LifeAndConductSigRes = @LifeAndConductSignatureDate where RciId in @RciIds"); }

                var inputParams = new
                {
                    ResidentCheckinDate = residentCheckinDate,
                    RaCheckinDate = raCheckinDate,
                    RdCheckinDate = rdCheckinDate,
                    LifeAndConductSignatureDate = lifeAndConductStatementCheckinDate,
                    RciIds = rciIds
                };

                connection.Execute(updateSql.ToString(), inputParams);
            }
        }

        public void SetRciCheckinGordonIdColumns(IEnumerable<int> rciIds, string checkinRaGordonId, string checkingRdGordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = new StringBuilder();

                if (checkinRaGordonId != null) { updateSql.AppendLine("update Rci set CheckinSigRAGordonID = @CheckinRaGordonId where RciId in @RciIds"); }
                if (checkingRdGordonId != null) { updateSql.AppendLine("update Rci set CheckinSigRDGordonID = @CheckinRdGordonId where RciId in @RciIds"); }

                var inputParams = new
                {
                    CheckinRaGordonId = checkinRaGordonId,
                    CheckinRdGordonId = checkingRdGordonId,
                    RciIds = rciIds
                };

                connection.Execute(updateSql.ToString(), inputParams);
            }
        }

        public void DeleteRci(int rciId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var deleteSql = "delete from Rci where RciId = @RciId";

                connection.Execute(deleteSql, new { RciId = rciId });
            }
        }

        public int CreateNewDamage(string description, string imagepath, int rciId, string gordonId, int roomComponentTypeId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var insertSql = Sql.Damage.DamageInsertStatement;

                string damageType = string.Empty;

                if (description != null)
                {
                    damageType = Constants.DAMAGE_TYPE_TEXT;
                }
                else
                {
                    damageType = Constants.DAMAGE_TYPE_IMAGE;
                }

                var inputParams = new
                {
                    DamageDescription = description,
                    DamageImagePath = imagepath,
                    DamageType = damageType,
                    RciId = rciId,
                    GordonId = gordonId,
                    RoomComponentTypeId = roomComponentTypeId
                };

                var insertedId = connection.Query<int>(insertSql, inputParams).Single();

                return insertedId;
            }
        }

        public Damage FetchDamageById(int damageId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Damage.SimpleDamageSelectStatement + "where damage.DamageId = @DamageId";

                var damage = connection.Query<Damage>(sql, new { DamageId = damageId }).Single();

                return damage;
            }
        }

        public void UpdateDamage(int damageId, string description, string imagepath, int? rciId, int? roomComponentTypeId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                bool allAreNull = description == null && imagepath == null && rciId == null && roomComponentTypeId == null;

                // Nothing to do
                if (allAreNull) { return; }

                var fieldUpdateStatements = new List<string>();

                if (description != null) { fieldUpdateStatements.Add("DamageDescription = @Description"); }
                if (imagepath != null) { fieldUpdateStatements.Add("DamageImagePath = @ImagePath"); }
                if (rciId != null) { fieldUpdateStatements.Add("RciId = @RciId"); }
                if (roomComponentTypeId != null) { fieldUpdateStatements.Add("RoomComponentTypeId = @RoomComponentTypeId"); }

                var updateSql = $"update Damage Set {string.Join(",", fieldUpdateStatements)} where DamageId = @DamageId";

                var inputparams = new
                {
                    Description = description,
                    ImagePath = imagepath,
                    RciId = rciId,
                    RoomComponentTypeId = roomComponentTypeId,
                    DamageId = damageId
                };

                connection.Execute(updateSql, inputparams);
            }
        }

        public void DeleteDamage(int damageId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var deleteDamageSql = "delete from Damage where DamageID = @DamageId";

                connection.Execute(deleteDamageSql, new { DamageId = damageId });
            }
        }

        public CommonAreaRciSignature CreateNewCommonAreaRciSignature(string gordonId, int rciId, DateTime signatureDate, string signatureType)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                // Since this table doesn't have a primary key (it has a composite key), just select the just inserted record and return it
                var insertSql = Sql.CommonAreaRciSignature.InsertStatment +
                    Sql.CommonAreaRciSignature.SimpleSelectStatement +  "where RciID = @RciId and GordonID = @GordonId and SignatureType = @SignatureType";

                var inputParams = new
                {
                    GordonId = gordonId,
                    RciId = rciId,
                    Signature = signatureDate,
                    SignatureType = signatureType
                };

                var insertedRecord = connection.Query<CommonAreaRciSignature>(insertSql, inputParams).Single();

                return insertedRecord;
            }
        }

        public void DeleteCommonAreaRciSignature(string gordonId, int rciId, string signatureType)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var deleteSql = "delete from CommonAreaRciSignature where RciID = @RciId and GordonID = @GordonId and SignatureType = @SignatureType";

                var inputParams = new
                {
                    GordonId = gordonId,
                    RciId = rciId,
                    SignatureType = signatureType
                };

                connection.Execute(deleteSql, inputParams);
            }
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
                var rciSql = Sql.Rci.RciSelectstatement + "where rci.RciId = @RciId";

                var damageSql = Sql.Damage.SimpleDamageSelectStatement + "where damage.RciId = @RciId";

                var fineSql = Sql.Fine.FineSelectStatement + "where fine.RciId = @RciId";

                var roomComponentTypesSql = Sql.RciTypeRoomComponentTypeMap.MapSelectStatment + "where rci.RciId = @RciId";

                var commonAreaRciSignaturesSql = Sql.CommonAreaRciSignature.SimpleSelectStatement + "where sig.RciId = @RciId";

                var sql = string.Join("\n\n", new List<string> { rciSql, damageSql, fineSql, roomComponentTypesSql, commonAreaRciSignaturesSql});

                var queryResult = connection.QueryMultiple(sql, new { RciId = rciId });

                BigRci rci;

                // The order in which the statements are made is the order you read them in.
                var allRciResults = queryResult.Read<BigRci>();

                try
                {
                    rci = allRciResults.Single();
                }
                catch (InvalidOperationException e)
                {
                    var errorMessage = $"Expected a single result to be returned for RciId {rciId}. Got {allRciResults.Count()}";

                    throw new Exception(errorMessage, e);
                }

                var damages = queryResult.Read<Damage>();

                var fines = queryResult.Read<Fine>();

                var roomComponentTypes = queryResult.Read<RoomComponentType>();

                var commonAreaSignatures = queryResult.Read<CommonAreaRciSignature>();

                rci.Damages = damages.ToList();

                rci.Fines = fines.ToList();

                rci.RoomComponentTypes = roomComponentTypes.ToList();

                rci.CommonAreaSignatures = commonAreaSignatures.ToList();

                // Also fetch Account Information for Checkin and Checkout Ra and Rd
                if (rci.CheckinRaGordonId != null) { rci.CheckinRaAccount = this.FetchAccountByGordonId(rci.CheckinRaGordonId); }
                if (rci.CheckinRdGordonId != null) { rci.CheckinRdAccount = this.FetchAccountByGordonId(rci.CheckinRdGordonId); }
                if (rci.CheckoutRaGordonId != null) { rci.CheckoutRaAccount = this.FetchAccountByGordonId(rci.CheckoutRaGordonId); }
                if (rci.CheckoutRdGordonId != null) { rci.CheckoutRdAccount = this.FetchAccountByGordonId(rci.CheckoutRdGordonId); }

                // Also fetch Account information for Apartment Mates in the case of an Apartment or Roommates in the case of a Dorm
                var normalizedRoomNumber = rci.RoomNumber.TrimEnd(Constants.ROOM_NUMBER_SUFFIXES);

                rci.RoomOrApartmentResidents = this.FetchResidentAccounts(rci.BuildingCode, normalizedRoomNumber, rci.SessionCode);

                return rci;
            }
        }

        public List<SmolRci> FetchRcisByGordonId(string gordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement + "where rci.GordonId = @GordonId";

                var queryResult = connection.Query<SmolRci>(sql, new { GordonId = gordonId });

                return queryResult.ToList();
            }
        }

        public List<SmolRci> FetchRcisByBuilding(List<string> buildings)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement +
                    "where rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query<SmolRci>(sql, new { BuildingCodes = buildings });

                return queryResult.ToList();
            }
        }

        public List<SmolRci> FetchRcisBySessionAndBuilding(List<string> sessions, List<string> buildings)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement +
                    "where rci.SessionCode in @SessionCodes and rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query<SmolRci>(sql, new { SessionCodes = sessions, BuildingCodes = buildings });

                return queryResult.ToList();
            }
        }

        public List<FineSummary> FetchFinesByBuilding(List<string> buildings)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Fine.FineSummarySelectStatement +
                    "where rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query<FineSummary>(sql, new { BuildingCodes = buildings }).ToList();

                return queryResult;
            }
        }

        public Account FetchAccountByGordonId(string gordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Account.AccountSelectStatement +
                    "where account.ID_NUM = @Id";

                var queryResult = connection.Query<Account>(sql, new { Id = gordonId });

                Account account = null;

                try
                {
                    account = queryResult.Single();
                }
                catch (InvalidOperationException e)
                {
                    if (queryResult.Count() == 0)
                    {
                        // User was not found, probably graduated. Set Gordon Id
                        account = new Account
                        {
                            GordonId = gordonId
                        };
                    }
                    else
                    {
                        throw new Exception($"Excpected exactly one account result, got {queryResult.Count()}", e);
                    }
                }

                return account;
            }
        }

        public List<Account> FetchResidentAccounts(string builingCode, string roomNumber, string sessionCode)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Account.ResidentAccountsSelectStatement +
                    "where roomAssign.BLDG_CDE = @BuildingCode and roomAssign.ROOM_CDE like '%' + @RoomNumber + '%' and roomAssign.SESS_CDE = @SessionCode";

                var inputParams = new
                {
                    BuildingCode = builingCode,
                    RoomNumber = roomNumber,
                    SessionCode = sessionCode
                };

                var queryResults = connection.Query<Account>(sql, inputParams).ToList();

                return queryResults;
            }
        }

        public List<SmolRci> FetchRcisForRoom(string building, string room)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement +
                    "where rci.BuildingCode = @Building and rci.RoomNumber = @Room";

                var queryResult = connection.Query<SmolRci>(sql, new { Building = building, Room = room });

                return queryResult.ToList();
            }
        }

        public Room FetchRoom(string buildingCode, string roomNumber)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Room.RoomSelectStatement +
                    "where room.BLDG_CDE = @BuildingCode and room.ROOM_CDE = @RoomNumber";

                var queryResult = connection.Query<Room>(sql, new { BuildingCode = buildingCode, RoomNumber = roomNumber });

                try
                {
                    var room = queryResult.First();

                    room.BuildingCode = room.BuildingCode.Trim();

                    room.RoomNumber = room.RoomNumber.Trim();

                    return room;
                }
                catch (InvalidOperationException ex)
                {
                    var errorMessage = $"Expected a single result to be returned for BuildingCode {buildingCode} and RoomNumber {roomNumber}. Got {queryResult.Count()}";

                    throw new Exception(errorMessage, ex);
                }
            }
        }

        public List<RoomComponentType> FetchRoomComponentTypesForRci(int rciId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.RciTypeRoomComponentTypeMap.MapSelectStatment +
                    "where rci.RciId = @RciId";

                var queryResults = connection.Query<RoomComponentType>(sql, new { RciId = rciId });

                return queryResults.ToList();
            }
        }

        public RoomAssignment FetchLatestRoomAssignmentForId(string id)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.RoomAssign.RoomAssignmentSelectStatment +
                    "where ID_NUM = @Id order by ASSIGN_DTE desc";

                var queryResult = connection.Query<RoomAssignment>(sql, new { Id = id }).ToList();

                var latestRoomAssign = queryResult.FirstOrDefault();

                // For now, we are ok with returning nulls
                return latestRoomAssign;
            }
        }

        public List<RoomAssignment> FetchRoomAssignmentsThatDoNotHaveRcis(string buildingCode, string sessionCode)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = "FindMissingRcis @Building, @CurrentSession";

                var inputParams = new
                {
                    Building = buildingCode,
                    CurrentSession = sessionCode
                };

                var queryResult = connection.Query<Models.RoomAssign>(sql, inputParams);

                var returnValue = new List<RoomAssignment>();

                foreach (var roomAssign in queryResult)
                {
                    returnValue.Add(
                        new RoomAssignment
                        {
                            GordonId = Convert.ToString(roomAssign.ID_NUM),
                            BuildingCode = roomAssign.BLDG_CDE?.Trim(),
                            RoomNumber = roomAssign.ROOM_CDE?.Trim(),
                            SessionCode = roomAssign.SESS_CDE?.Trim(),
                            AssignmentDate = roomAssign.ASSIGN_DTE,
                            RoomType = roomAssign.ROOM_TYPE
                        });
                }

                return returnValue;
            }
        }
    }
}