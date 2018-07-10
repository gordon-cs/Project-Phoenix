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
                var sql = Sql.Rci.BigRciSelectStatement + "where rci.RciId = @RciId";

                var queryResult = connection.Query(sql, new { RciId = rciId });

                var mapperResult = Slapper.AutoMapper.MapDynamic<BigRci>(queryResult);

                BigRci retVal;

                try
                {
                    retVal = mapperResult.Single();
                }
                catch (InvalidOperationException e)
                {
                    var errorMessage = $"Expected a single result to be returned for RciId {rciId}. Got {mapperResult.Count()}";

                    throw new Exception(errorMessage, e);
                }

                return retVal;
            }
        }

        public List<BigRci> FetchRcisByGordonId(string gordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.BigRciSelectStatement + "where rci.GordonId = @GordonId";

                var queryResult = connection.Query(sql, new { GordonId = gordonId });

                var mapperResult = Slapper.AutoMapper.MapDynamic<BigRci>(queryResult).ToList();

                return mapperResult.ToList();
            }
        }

        public List<SmolRci> FetchRcisByBuilding(List<string> buildings)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.SmolRciSelectstatement +
                    "where rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query(sql, new { BuildingCodes = buildings });

                var mapperResult = Slapper.AutoMapper.MapDynamic<SmolRci>(queryResult).ToList();

                return mapperResult;
            }
        }

        public List<SmolRci> FetchRcisBySessionAndBuilding(List<string> sessions, List<string> buildings)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.SmolRciSelectstatement +
                    "where rci.SessionCode in @SessionCodes and rci.BuildingCode in @BuildingCodes";

                var queryResult = connection.Query(sql, new { SessionCodes = sessions, BuildingCodes = buildings });

                var mapperResult = Slapper.AutoMapper.MapDynamic<SmolRci>(queryResult).ToList();

                return mapperResult;
            }
        }

        public Account FetchAccountByGordonId(string gordonId)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Account.AccountSelectStatement +
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

        public List<SmolRci> FetchRcisForRoom(string building, string room)
        {
            using (var connection = this._dbConnectionFactory.CreateConnection())
            {
                var sql = Sql.Rci.SmolRciSelectstatement +
                    "where rci.BuildingCode = @Building and rci.RoomNumber = @Room";

                var queryResult = connection.Query(sql, new { Building = building, Room = room });

                var mapperResult = Slapper.AutoMapper.MapDynamic<SmolRci>(queryResult).ToList();

                return mapperResult;
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
    }
}