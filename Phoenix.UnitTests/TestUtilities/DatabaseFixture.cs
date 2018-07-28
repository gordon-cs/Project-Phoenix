using Phoenix.DapperDal;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Phoenix.UnitTests.TestUtilities
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            this.DbFactory = new TestDbConnectionFactory();
        }

        public void Dispose()
        {
            // Nothing to dispose of here
        }

        private IDbConnection Db { get; set; }

        public IDbConnectionFactory DbFactory { get; set; }
    }

    public class TestDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RCIDatabase"].ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
