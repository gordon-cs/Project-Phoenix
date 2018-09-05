using Dapper;
using Phoenix.DapperDal.Types;
using Phoenix.Exceptions;
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
    public class DapperDal : IDatabaseDal
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
            if (rciIds.Count() <= 0)
            {
                return;
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = "update Rci set IsCurrent = @IsCurrent where RciId in @RciIds";

                connection.Execute(updateSql, new { IsCurrent = isCurrent, RciIds = rciIds });
            }
        }

        public void SetRciGordonIdColumn(IEnumerable<int> rciIds, string gordonId)
        {
            if (rciIds.Count() <= 0)
            {
                return;
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = "update Rci set GordonID = @GordonId where RciId in @RciIds";

                connection.Execute(updateSql, new { GordonId = gordonId, RciIds = rciIds });
            }
        }

        public void SetRciCheckinDateColumns(IEnumerable<int> rciIds, DateTime? residentCheckinDate, DateTime? raCheckinDate, DateTime? rdCheckinDate, DateTime? lifeAndConductStatementCheckinDate)
        {
            if (rciIds.Count() <= 0)
            {
                return;
            }

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
            if (rciIds.Count() <= 0)
            {
                return;
            }

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

        public void SetRciCheckoutDateColumns(IEnumerable<int> rciIds, DateTime? residentCheckoutDate, DateTime? raCheckoutDate, DateTime? rdCheckoutDate)
        {
            if (rciIds.Count() <= 0)
            {
                return;
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = new StringBuilder();

                if (residentCheckoutDate != null) { updateSql.AppendLine($"update Rci set CheckoutSigRes = @ResidentCheckoutDate where RciId in @RciIds"); }
                if (raCheckoutDate != null) { updateSql.AppendLine($"update Rci set CheckoutSigRA = @RaCheckoutDate where RciId in @RciIds"); }
                if (rdCheckoutDate != null) { updateSql.AppendLine($"update Rci set CheckoutSigRD = @RdCheckoutDate where RciId in @RciIds"); }

                var inputParams = new
                {
                    ResidentCheckoutDate = residentCheckoutDate,
                    RaCheckoutDate = raCheckoutDate,
                    RdCheckoutDate = rdCheckoutDate,
                    RciIds = rciIds
                };

                connection.Execute(updateSql.ToString(), inputParams);
            }
        }

        public void SetRciCheckoutGordonIdColumns(IEnumerable<int> rciIds, string checkoutRaGordonId, string checoutRdGordonId)
        {
            if (rciIds.Count() <= 0)
            {
                return;
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var updateSql = new StringBuilder();

                if (checkoutRaGordonId != null) { updateSql.AppendLine("update Rci set CheckoutSigRAGordonID = @CheckoutRaGordonId where RciId in @RciIds"); }
                if (checoutRdGordonId != null) { updateSql.AppendLine("update Rci set CheckoutSigRDGordonID = @CheckoutRdGordonId where RciId in @RciIds"); }

                var inputParams = new
                {
                    CheckoutRaGordonId = checkoutRaGordonId,
                    CheckoutRdGordonId = checoutRdGordonId,
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

                try
                {
                    var damage = connection.Query<Damage>(sql, new { DamageId = damageId }).Single();

                    return damage;
                }
                catch (InvalidOperationException ex)
                {
                    throw new DamageNotFoundException($"Could not find damage with id {damageId}", ex);
                }
            }
        }

        public void UpdateDamage(IEnumerable<int> damageIds, string description, string imagepath, int? rciId, int? roomComponentTypeId)
        {
            if (damageIds.Count() <= 0)
            {
                return;
            }

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

                var updateSql = $"update Damage Set {string.Join(",", fieldUpdateStatements)} where DamageId in @DamageIds";

                var inputparams = new
                {
                    Description = description,
                    ImagePath = imagepath,
                    RciId = rciId,
                    RoomComponentTypeId = roomComponentTypeId,
                    DamageIds = damageIds
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

        public Fine FetchFineById(int fineId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Fine.FineSelectStatement + "where fine.FineId = @FineId";

                try
                {
                    var fine = connection.Query<Fine>(sql, new { FineId = fineId }).Single();

                    return fine;
                }
                catch (InvalidOperationException ex)
                {
                    throw new FineNotFoundException($"Could not find fine with id {fineId}", ex);
                }
            }
        }

        public int CreateNewFine(decimal amount, string gordonId, string reason, int rciId, int roomComponentTypeId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var insertSql = Sql.Fine.FineInsertStatement;

                var inputParams = new
                {
                    Amount = amount,
                    GordonId = gordonId,
                    Reason = reason,
                    RciId = rciId,
                    RoomComponentTypeId = roomComponentTypeId
                };

                var fineId = connection.Query<int>(insertSql, inputParams).Single();

                return fineId;
            }
        }

        public void UpdateFine(IEnumerable<int> fineIds, decimal? amount, string gordonId, string reason, int? rciId, int? roomComponentTypeId)
        {
            if (fineIds.Count() <= 0)
            {
                return;
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                bool allAreNull = amount == null && gordonId == null && reason == null && rciId == null && roomComponentTypeId == null;

                if (allAreNull) { return; }

                var fieldUpdateStatements = new List<string>();

                if (amount != null) { fieldUpdateStatements.Add("FineAmount = @Amount"); }
                if (gordonId != null) { fieldUpdateStatements.Add("GordonID = @GordonId"); }
                if (reason != null) { fieldUpdateStatements.Add("Reason = @Reason"); }
                if (rciId != null) { fieldUpdateStatements.Add("RciId = @RciId"); }
                if (roomComponentTypeId != null) { fieldUpdateStatements.Add("RoomComponentTypeId = @RoomComponentTypeId"); };

                var updateSql = $"update Fine set {string.Join(",", fieldUpdateStatements)} where FineID in @FineIds";

                var inputParams = new
                {
                    Amount = amount,
                    GordonId = gordonId,
                    Reason = reason,
                    RciId = rciId,
                    RoomComponentTypeId = roomComponentTypeId,
                    FineIds = fineIds
                };

                connection.Execute(updateSql, inputParams);
            }
        }

        public void DeleteFine(int fineId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var deleteFineSql = "delete from Fine where FineID = @FineId";

                connection.Execute(deleteFineSql, new { FineId = fineId });
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

        public List<string> FetchBuildingCodes()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = @"select BuildingCode from BuildingAssign group by BuildingCode";

                return connection.Query<string>(sql).ToList();
            }
        }

        public List<ResidentHallGrouping> FetchBuildingMap()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = @"select JobTitleHall as HallGroup, BuildingCode as BuildingCode from BuildingAssign";

                var queryResult = connection.Query(sql).ToList();

                var dict = new Dictionary<string, List<string>>();

                foreach (dynamic row in queryResult)
                {
                    if (dict.ContainsKey(Convert.ToString(row.HallGroup)))
                    {
                        dict[Convert.ToString(row.HallGroup)].Add(Convert.ToString(row.BuildingCode));
                    }
                    else
                    {
                        dict[Convert.ToString(row.HallGroup)] = new List<string> { Convert.ToString(row.BuildingCode) };
                    }
                }

                return dict.Select(x => new ResidentHallGrouping { HallGroup = x.Key, BuildingCodes = x.Value }).ToList();
            }
        }

        public Dictionary<string, string> FetchBuildingCodeToBuildingNameMap()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Room.BuildingCodeToBuildingMapSelectStatement;

                var map = connection.Query<Building>(sql);

                return map.ToDictionary(x => x.BuildingCode.Trim(), y => y.BuildingDescription.Trim());
            }
        }

        public List<Session> FetchSessions()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = @"select RTRIM(s.SESS_CDE) as SessionCode, 
                            s.SESS_DESC as SessionDescription, 
                            s.SESS_BEGN_DTE as SessionStartDate, 
                            s.SESS_END_DTE as SessionEndDate
                            from session as s";

                return connection.Query<Session>(sql).ToList();
            }
        }

        public string FetchCurrentSession()
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var currentSessionSql = "GetCurrentSession";

                try
                {
                    var currentSession = connection.Query<string>(currentSessionSql).Single();

                    return currentSession.Trim();
                }
                catch (InvalidOperationException ex)
                {
                    throw new CurrentSessionNotFoundException($"Could not get current session!", ex);
                }
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

                    throw new RciNotFoundException(errorMessage, e);
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
                // If the fields are not null, but we are unable to find the account in the accounts table,
                // it probably means the user is an alumni. So we just set the account id.
                try
                {
                    if (rci.CheckinRaGordonId != null) { rci.CheckinRaAccount = this.FetchAccountByGordonId(rci.CheckinRaGordonId); }
                }
                catch (UserNotFoundException)
                {
                    rci.CheckinRaAccount = new Account { GordonId = rci.CheckinRaGordonId };
                }
                try
                {
                    if (rci.CheckinRdGordonId != null) { rci.CheckinRdAccount = this.FetchAccountByGordonId(rci.CheckinRdGordonId); }
                }
                catch (UserNotFoundException)
                {
                    rci.CheckinRdAccount = new Account { GordonId = rci.CheckinRdGordonId };
                }
                try
                {
                    if (rci.CheckoutRaGordonId != null) { rci.CheckoutRaAccount = this.FetchAccountByGordonId(rci.CheckoutRaGordonId); }
                }
                catch (UserNotFoundException)
                {
                    rci.CheckoutRaAccount = new Account { GordonId = rci.CheckoutRaGordonId };
                }
                try
                {
                    if (rci.CheckoutRdGordonId != null) { rci.CheckoutRdAccount = this.FetchAccountByGordonId(rci.CheckoutRdGordonId); }
                }
                catch (UserNotFoundException)
                {
                    rci.CheckoutRdAccount = new Account { GordonId = rci.CheckoutRdGordonId };
                }

                // Also fetch Account information for Apartment Mates in the case of an Apartment or Roommates in the case of a Dorm
                var normalizedRoomNumber = rci.RoomNumber.TrimEnd(Constants.ROOM_NUMBER_SUFFIXES);

                rci.RoomOrApartmentResidents = this.FetchResidentAccounts(rci.BuildingCode, normalizedRoomNumber, rci.SessionCode);

                return rci;
            }
        }

        public List<SmolRci> FetchRcisById(IEnumerable<int> rciIds)
        {
            if (rciIds.Count() <= 0)
            {
                return new List<SmolRci>();
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement + "where rci.RciId in @RciIds";

                var queryResult = connection.Query<SmolRci>(sql, new { RciIds = rciIds });

                return queryResult.ToList();
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

        public List<SmolRci> FetchRcisByBuilding(IEnumerable<string> buildings)
        {
            if (buildings.Count() <= 0)
            {
                return new List<SmolRci>();
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement +
                    "where rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query<SmolRci>(sql, new { BuildingCodes = buildings });

                return queryResult.ToList();
            }
        }

        public List<SmolRci> FetchRcisBySessionAndBuilding(IEnumerable<string> sessions, IEnumerable<string> buildings)
        {
            if (buildings.Count() <= 0 && sessions.Count() <= 0)
            {
                return new List<SmolRci>();
            }

            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.RciSelectstatement +
                    "where rci.SessionCode in @SessionCodes and rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query<SmolRci>(sql, new { SessionCodes = sessions, BuildingCodes = buildings });

                return queryResult.ToList();
            }
        }

        public List<FineSummary> FetchFinesByBuilding(IEnumerable<string> buildings)
        {
            if (buildings.Count() <= 0)
            {
                return new List<FineSummary>();
            }

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

                    return account;
                }
                catch (InvalidOperationException e)
                {
                    throw new UserNotFoundException($"User with gordon id={gordonId} was not found in the Accounts table.", e);
                }
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

                    throw new RoomNotFoundException(errorMessage, ex);
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

        public RoomAssignment FetchRoomAssignmentForId(string id, string sessionCode)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.RoomAssign.RoomAssignmentSelectStatment +
                    "where ID_NUM = @Id and SESS_CDE = @SessionCode";

                var queryResult = connection.Query<RoomAssignment>(sql, new { Id = id, SessionCode = sessionCode}).ToList();

                try
                {
                    var latestRoomAssign = queryResult.First();

                    return latestRoomAssign;
                }
                catch (InvalidOperationException)
                {
                    throw new RoomAssignNotFoundException();
                }
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