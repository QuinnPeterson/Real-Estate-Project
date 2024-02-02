using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using System.Collections.Generic;

namespace Server.Controllers
{

    [EnableCors("MyAllowSpecificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }



        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest model)
        {
            var response = await _authService.Login(model);

            if (response.ErrorMessage != null)
            {
                return BadRequest(new { error = response.ErrorMessage });
            }

            return Ok(response);
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] string username, [FromForm] IFormFile avatar, [FromForm] string email, [FromForm] string password)
        {
            var registrationResult = await _authService.Register(username, avatar, email, password);

            if (registrationResult.IsSuccess)
            {
                return Ok(new { message = "Registration successful" });
            }
            else
            {
                return ValidationProblem(registrationResult.ErrorMessage);
            }
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                IEnumerable<User> allUsers = await _authService.GetAllUsersAsync();
                User user = allUsers.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }




}

