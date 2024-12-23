using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
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
    }
} 