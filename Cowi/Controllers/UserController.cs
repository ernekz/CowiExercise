using Cowi.DTO;
using Cowi.Models;
using Cowi.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Cowi.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// Create a User 
        /// </summary>
        /// <remarks>
        /// There is no regulations for password nor email.
        /// Parameters cannot be null
        /// 
        /// Sample request:
        /// 
        ///     POST /register
        ///     {
        ///         "email": "Test1",
        ///         "password": "test1234"
        ///     }
        ///     
        ///      POST /register
        ///     {
        ///         "email": "Test@gmail.com",
        ///         "password": "Test1234*"
        ///     }
        /// </remarks>
        /// <param name="registrationDTO"> The user's register credentials</param>
        /// <returns>Returns a message upon successful registration</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegistrationDTO registrationDTO)
        {
            
            var newUser = await _userService.CreateUserAsync(registrationDTO);

            return CreatedAtAction(nameof(Register), new { email = newUser.Email }, $"User with email {newUser.Email} created successfully");
        }

        /// <summary>
        /// Authenticates a user and returns a token
        /// </summary>
        /// <remarks>
        ///
        /// 
        /// Sample request:
        /// 
        ///     POST /login
        ///     {
        ///         "email": "Test1",
        ///         "password": "test1234"
        ///     }
        ///
        /// Sample response (200 OK):
        ///
        ///     {
        ///         "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
        ///     }
        ///     
        /// </remarks>
        /// <param name="userLoginDTO"> The user's login credentials.</param>
        /// <returns>Returns a token upon successful authentication</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {

            string token = await _userService.AuthenticateUserAsync(userLoginDTO);
            
            if (token != null)
            {
                return Ok(new {Token = token});
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
