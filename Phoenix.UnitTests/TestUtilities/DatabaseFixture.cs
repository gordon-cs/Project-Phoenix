using Dapper;
using Phoenix.DapperDal;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Phoenix.UnitTests.TestUtilities
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            //// Create a connection to the master database since the RCITrain database may not exist yet.
            //this.Db = new SqlConnection(ConfigurationManager.ConnectionStrings["RCIDatabase"].ConnectionString);

            //this.Db.Open();

            ///* Drop Test Database if it exists */
            //var dropDatabaseSql = @"
            //        IF EXISTS (SELECT name FROM master.sys.databases WHERE name = N'RCITrain')
            //            BEGIN
            //                ALTER DATABASE [RCITrain] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
            //                DROP DATABASE [RCITrain]
            //            END";

            //this.Db.Query(dropDatabaseSql);

            ///* Re- Create Database */
            //var createDatabaseSql = @"
            //        CREATE DATABASE [RCITrain]";

            //this.Db.Execute(createDatabaseSql);


            ///* Create Schema and Populate with test data */
            //var sqlSchemaFileStream = new FileStream("TestDatabase\\Schema.sql", FileMode.Open);
            //var dataFileStream = new FileStream("TestDatabase\\Data.sql", FileMode.Open);

            //using (var schemaStreamReader = new StreamReader(sqlSchemaFileStream))
            //using (var dataStreamReader = new StreamReader(dataFileStream))
            //{
            //    var schemaSql = schemaStreamReader.ReadToEnd();
            //    var dataSql = dataStreamReader.ReadToEnd();

            //    this.Db.Execute(schemaSql);
            //    this.Db.Execute(dataSql);
            //}

            //// And we are done with this sql connection, close it.
            //this.Db.Close();

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
