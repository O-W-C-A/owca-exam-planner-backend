using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using API.Data;
using API.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration; // Injected for secret key access

        public UserController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("auth/login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginUser loginUser)
        {
            if (loginUser == null)
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                // Validate user credentials
                var user = await _context.Users
                    .FirstOrDefaultAsync(e =>
                        e.Email == loginUser.Email &&
                        e.PasswordHash == loginUser.PasswordHash);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                // Return token and user info
                return Ok(new
                {
                    token,
                    role = user.Role,
                    userId = user.UserID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]); // Use a strong secret key from config

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role) // Role claim
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Token expires in 2 hours
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
