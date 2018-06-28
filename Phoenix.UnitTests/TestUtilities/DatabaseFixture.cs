using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.UnitTests.TestUtilities
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            this.Db = new SqlConnection("Server=(LocalDB)\\MSSQLLocalDB; Integrated Security=True; MultipleActiveResultSets=True");

            /* Drop Test Database if it exists */
            var dropDatabaseSql = @"
                    IF EXISTS (SELECT name FROM master.sys.databases WHERE name = N'RCITrain')
                        BEGIN
                            ALTER DATABASE [RCITrain] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                            DROP DATABASE [RCITrain]
                        END";

            this.Db.Query(dropDatabaseSql);

            /* Re- Create Database */
            var createDatabaseSql = @"
                    CREATE DATABASE [RCITrain]";

            this.Db.Execute(createDatabaseSql);


            /* Create Schema and Populate with test data */
            var sqlSchemaFileStream = new FileStream("TestDatabase\\Schema.sql", FileMode.Open);
            var dataFileStream = new FileStream("TestDatabase\\Data.sql", FileMode.Open);

            using (var schemaStreamReader = new StreamReader(sqlSchemaFileStream))
            using (var dataStreamReader = new StreamReader(dataFileStream))
            {
                var schemaSql = schemaStreamReader.ReadToEnd();
                var dataSql = dataStreamReader.ReadToEnd();

                this.Db.Execute(schemaSql);
                this.Db.Execute(dataSql);
            }
        }

        public void Dispose()
        {
            this.Db.Close();
        }

        public readonly SqlConnection Db { get; set; }
    }


}
