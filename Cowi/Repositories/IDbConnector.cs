using System.Data.SqlClient;

namespace Cowi.Repositories
{
    public interface IDbConnector
    {

        Task<SqlConnection> OpenConnectionAsync();
    }
}
