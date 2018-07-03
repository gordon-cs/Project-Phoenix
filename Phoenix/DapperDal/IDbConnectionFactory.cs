using System.Data;

namespace Phoenix.DapperDal
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}