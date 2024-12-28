using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public CourseController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet("course/group/{groupId}")]
        public async Task<IActionResult> GetCoursesByGroup(int groupId)
        {
            try
            {
                // Get group with its specialization
                var group = await _context.Groups
                    .Include(g => g.Specialization)
                    .FirstOrDefaultAsync(g => g.GroupID == groupId);

                if (group == null)
                {
                    return NotFound($"Group with ID {groupId} not found.");
                }

                // Get courses for the group's specialization
                var courses = await _context.Courses
                    .Where(c => c.SpecializationID == group.SpecializationID)
                    .Select(c => new
                    {
                        id = c.CourseID,
                        name = c.Title
                    })
                    .ToListAsync();

                if (!courses.Any())
                {
                    return NotFound($"No courses found for group {group.Name}");
                }

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("course/professor/{userId}")]
        public async Task<IActionResult> GetCoursesByProfessor(int userId)
        {
            try
            {
                // First get the professor record for this user
                var professor = await _context.Professors
                    .FirstOrDefaultAsync(p => p.UserID == userId);

                if (professor == null)
                {
                    return NotFound($"No professor found for user ID {userId}");
                }

                // Get all courses where this professor is the main professor
                var mainCourses = await _context.Courses
                    .Where(c => c.ProfessorID == professor.ProfessorID)
                    .Select(c => new
                    {
                        id = c.CourseID,
                        name = c.Title
                    })
                    .ToListAsync();

                // Get all courses where this professor is a lab holder
                var labCourses = await _context.LabHolders
                    .Where(lh => lh.ProfessorID == professor.ProfessorID)
                    .Include(lh => lh.Course)
                    .Select(lh => new
                    {
                        id = lh.Course.CourseID,
                        name = lh.Course.Title
                    })
                    .ToListAsync();

                // Combine and remove duplicates
                var allCourses = mainCourses
                    .Union(labCourses)
                    .OrderBy(c => c.name)
                    .ToList();

                if (!allCourses.Any())
                {
                    return NotFound($"No courses found for professor with ID {userId}");
                }

                return Ok(allCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 