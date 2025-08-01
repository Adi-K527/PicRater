using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(UserContext context, IConfiguration configuration, ILogger<UserController> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        // GET: api/User/health
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Healthy" });
        }

        // GET: api/User/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserModel(int id)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "user_id");

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = int.Parse(userIdClaim.Value);
                if (userId != id)
                {
                    _logger.LogWarning("User with ID {UserId} attempted to access user with ID {TargetUserId}", userId, id);
                    return Forbid();
                }

                var userModel = await _context.UserModel.FindAsync(id);

                if (userModel == null)
                {
                    return NotFound();
                }

                _logger.LogInformation("Request {Method} {Path} User with ID {UserId} retrieved successfully", HttpMethods.Get, HttpContext.Request.Path, id);
                return StatusCode(200, new { user = userModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                return StatusCode(500, $"Error retrieving user");
            }
        }

        // PUT: api/User/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserModel(UpdateUserDto userModel, int id)
        {
            try
            {
                var existingUser = await _context.UserModel.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Username = userModel.Username ?? existingUser.Username;
                existingUser.Password = userModel.Password ?? existingUser.Password;
                existingUser.FirstName = userModel.FirstName ?? existingUser.FirstName;
                existingUser.LastName = userModel.LastName ?? existingUser.LastName;

                _context.Entry(existingUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Request {Method} {Path} User with ID {UserId} updated successfully", HttpMethods.Put, HttpContext.Request.Path, id);
                return StatusCode(200, new { user = existingUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, $"Error updating user");
            }

        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> LoginUser(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _context.UserModel
                                    .FirstOrDefaultAsync(u => u.Username == loginUserDto.Username &&
                                                              u.Password == loginUserDto.Password);

                if (user == null)
                {
                    return Unauthorized();
                }

                var user_token = GenerateToken(user.UserId);

                _logger.LogInformation("User {Username} logged in successfully", user.Username);
                _logger.LogInformation("Generated token for user {Username}", user.Username);
                return StatusCode(200, new { message = "success", user = user, token = user_token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in");
                return StatusCode(500, $"Error logging in");
            }
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> RegisterUser(RegisterUserDto registerUserDto)
        {
            try
            {
                var userModel = new UserModel
                {
                    Username = registerUserDto.Username,
                    Password = registerUserDto.Password,
                    FirstName = registerUserDto.FirstName,
                    LastName = registerUserDto.LastName
                };

                _context.UserModel.Add(userModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Username} registered successfully", userModel.Username);
                _logger.LogInformation("Generated token for user {Username}", userModel.Username);
                return StatusCode(201, new { message = "success", user = userModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, $"Error registering user");
            }

        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserModel(int id)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "user_id");

                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = int.Parse(userIdClaim.Value);
                if (userId != id)
                {
                    _logger.LogWarning("User with ID {UserId} attempted to delete user with ID {TargetUserId}", userId, id);
                    return Forbid();
                }

                var userModel = await _context.UserModel.FindAsync(id);
                if (userModel == null)
                {
                    return NotFound();
                }

                _context.UserModel.Remove(userModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Request {Method} {Path} User with ID {UserId} deleted successfully", HttpMethods.Delete, HttpContext.Request.Path, id);
                _logger.LogInformation("User with ID {UserId} deleted successfully", id);
                return StatusCode(200, new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, $"Error deleting user");
            }
        }

        private string GenerateToken(int user_id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("Jwt__Secret") ?? throw new Exception("Missing JWT secret"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("user_id", user_id.ToString())
                }),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
