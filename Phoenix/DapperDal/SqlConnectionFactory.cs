using System.Data;
using System.Data.SqlClient;

namespace Phoenix.DapperDal
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var conn = new SqlConnection(this._connectionString);
            conn.Open();
            return conn;
        }
    }
}