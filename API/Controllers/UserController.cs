using API.Data;
using API.Enum;
using API.Mapping;
using API.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel;
namespace API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public UserController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost("api/auth/login")]
        public async Task<IActionResult> UserLogin(LoginUser loginUser)
        {
            if (loginUser == null)
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var user = await _context.Users
                .Where(e => e.Email == loginUser.Email && e.PasswordHash == loginUser.PasswordHash)
                .ToListAsync();

                if (user == null || user.Count == 0)
                {
                    return NotFound("User with these credentials not found");
                }

                if (user.Count > 1)
                {
                    throw new Exception("Too many users with the same name");
                }

                if (user[0].Email == loginUser.Email &&
                    user[0].PasswordHash == loginUser.PasswordHash)
                {
                    return Ok(new { Role = user[0].Role });
                }


                return BadRequest("Something unexpected happened");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
