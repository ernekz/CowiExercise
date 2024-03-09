using System.Data.SqlClient;

namespace Cowi.Repositories
{
    public class DbConnector : IDbConnector
    {
        private readonly string _connectionString;

        public DbConnector(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Cowi");
        }

        public async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
