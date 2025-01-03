using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using API.Data;
using API.Models;

using Microsoft.AspNetCore.Authorization;
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
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(e =>
                        e.Email == loginUser.Email &&
                        e.PasswordHash == loginUser.PasswordHash);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var claims = new List<Claim>
                {
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                // Only get student info if the user is a student
                if (user.Role == RoleEnum.Student)
                {
                    var studentInfo = await _context.Students
                        .Include(s => s.Group)
                        .FirstOrDefaultAsync(s => s.UserID == user.UserID);

                    if (studentInfo?.IsLeader == true)
                    {
                        claims.Add(new Claim("IsLeader", "true"));
                        claims.Add(new Claim("GroupID", studentInfo.GroupID.ToString()));
                    }

                    // Return student-specific response
                    return Ok(new
                    {
                        token = GenerateJwtToken(claims),
                        role = user.Role,
                        userId = user.UserID,
                        isLeader = studentInfo?.IsLeader ?? false,
                        groupId = studentInfo?.GroupID,
                        groupName = studentInfo?.Group?.Name
                    });
                }
                if(user.Role ==RoleEnum.Professor)
                {
                    var profInfo = await _context.Professors
                        .FirstOrDefaultAsync(s => s.UserID == user.UserID);

                    // Return prof-specific response
                    return Ok(new
                    {
                        token = GenerateJwtToken(claims),
                        role = user.Role,
                        userId = user.UserID,
                        profId = profInfo.ProfessorID,
                    });
                }

                // Return basic response for non-student users
                return Ok(new
                {
                    token = GenerateJwtToken(claims),
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
        private string GenerateJwtToken(List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Faculty)
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                // Get student info including group if the user is a student
                var studentInfo = await _context.Students
                    .Include(s => s.Group)
                    .FirstOrDefaultAsync(s => s.UserID == userId);

                var professorInfo = await _context.Professors
                    .Include(p => p.Department)
                    .FirstOrDefaultAsync(p => p.UserID == userId && p.DepartmentID != null);

                // Return user data (excluding sensitive information)
                return Ok(new
                {
                    firstname = user.FirstName,
                    lastname = user.LastName,
                    email = user.Email,
                    role = user.Role,
                    faculty = user.Faculty?.LongName,
                    group = studentInfo?.Group?.Name, // Will be null if user is not a student
                    department = professorInfo?.Department?.Name // Will be null if user is not a professor
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("users/professor/{userId}")]
        public async Task<IActionResult> GetProfessorDetails(int userId)
        {
            try
            {
                var professor = await _context.Professors
                    .Include(p => p.User)
                    .Include(p => p.Department)
                    .FirstOrDefaultAsync(p => p.UserID == userId);

                if (professor == null)
                {
                    return NotFound($"No professor found for user ID {userId}");
                }

                var professorDetails = new
                {
                    id = professor.ProfessorID.ToString(),
                    firstName = professor.User.FirstName,
                    lastName = professor.User.LastName,
                    email = professor.User.Email,
                    department = professor.Department?.Name ?? "No Department",
                    title = professor.Title
                };

                return Ok(professorDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("users/assistants/{courseId}")]
        public async Task<IActionResult> GetCourseAssistants(int courseId)
        {
            try
            {
                // Get lab holders (assistants) for the course
                var assistants = await _context.LabHolders
                    .Include(lh => lh.Professor)
                        .ThenInclude(p => p.User)
                    .Where(lh => lh.CourseID == courseId)
                    .Select(lh => new
                    {
                        id = lh.ProfessorID,
                        firstName = lh.Professor.User.FirstName,
                        lastName = lh.Professor.User.LastName,
                        fullName = $"{lh.Professor.User.FirstName} {lh.Professor.User.LastName}"
                    })
                    .ToListAsync();

                if (!assistants.Any())
                {
                    return NotFound($"No assistants found for course ID {courseId}");
                }

                return Ok(assistants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
