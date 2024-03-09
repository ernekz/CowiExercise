using Cowi.Models;
using System.Data.SqlClient;

namespace Cowi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnector _connector;

        public UserRepository(IDbConnector dbConnector)
        {
            _connector = dbConnector;
        }


        public async Task<User> CreateUserAsync(User user)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Insert into Users (Email, PasswordHash) Values (@Email, @Password);";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);

                    await command.ExecuteNonQueryAsync();

                    return user;
                }
            }
        }

        public async Task<User> DeleteUserAsync(string email)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Delete from Users Where Email = @Email;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return null;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Select * From users Where Email = @Email;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapUserFromDataReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Update Users Set PasswordHash = @Password Where Email = @Email;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);

                    await command.ExecuteNonQueryAsync(); ;
                }
            }
            return user;
        }
        private User MapUserFromDataReader(SqlDataReader reader)
        {
            return new User
            {
                Email = (string)reader["Email"],
                Password = (string)reader["PasswordHash"]
            };
        }
    }
}
