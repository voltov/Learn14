using System.Data;

namespace Task_Dapper
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}