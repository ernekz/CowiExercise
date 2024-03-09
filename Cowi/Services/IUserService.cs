using Cowi.DTO;
using Cowi.Models;

namespace Cowi.Services
{
    public interface IUserService
    {

        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(UserRegistrationDTO user);
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(string email);
        Task<string> AuthenticateUserAsync(UserLoginDTO userLogin);
    }
}
