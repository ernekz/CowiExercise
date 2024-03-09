using Cowi.DTO;
using Cowi.Helper;
using Cowi.Models;
using Cowi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Cowi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;
        private readonly IJwtHelper _jwtHelper;

        public UserService(IJwtHelper jwtHelper, IUserRepository userRepository, IConfiguration configuration, PasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _jwtHelper = jwtHelper;
        }

        public async Task<string> AuthenticateUserAsync(UserLoginDTO userLogin)
        {
            User user = await _userRepository.GetUserByEmailAsync(userLogin.Email);

            if (user == null || !VerifyPassword(userLogin.Password, user.Password))
            {
                return null;
            }

            string token = _jwtHelper.GenerateJwtToken(user);

            return token;
        }

        

        public async Task<User> CreateUserAsync(UserRegistrationDTO userDto)
        {
            var user = new User
            {
                Email = userDto.Email,
                Password = _passwordHasher.HashPassword(userDto.Password)
            };
            return await _userRepository.CreateUserAsync(user);
        }

        public Task<User> DeleteUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        private bool VerifyPassword(string loginPassword, string dbPassword)
        {
            var result = _passwordHasher.VerifyPassword(dbPassword, loginPassword);

            if (result == false)
            {
                return false;
            }
            return true;
        }
    }
}
