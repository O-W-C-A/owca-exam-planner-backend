using API.Data;
using API.Enum;
using API.Mapping;
using API.Models;
using API.Models.DTOmodels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace API.Controllers
{
    public class ExamController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly CourseMapper _courseMapper;

        // Constructor
        public ExamController(ApiDbContext context, CourseMapper courseMapper)
        {
            _context = context;
            _courseMapper = courseMapper;
        }

        [HttpGet("GetCoursersForExamByUserID")]
        public async Task<IActionResult> GetCoursersForExamByUserID(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Role == StatusUserEnum.Student)
            {
                var student = await _context.Students
                    .Include(s => s.Group)
                    .ThenInclude(g => g.Specialization)
                    .FirstOrDefaultAsync(s => s.UserID == userId);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                var specializationId = student.Group.SpecializationID;

                // Obținem cursurile pentru specializarea studentului
                var courses = await _context.Courses
                    .Where(c => c.SpecializationID == specializationId)
                    .Include(c => c.Professor)
                    .ThenInclude(c => c.User)
                    .ToListAsync();

                if (courses == null || !courses.Any())
                {
                    return NotFound("No courses found for this student.");
                }

                var examRequests = await _context.ExamRequests
                    .Where(er => er.GroupID == student.GroupID && er.Status=="Pending")
                    .Select(er => er.CourseID)
                    .ToListAsync();

                var availableCourses = courses
                    .Where(course => examRequests.Contains(course.CourseID))
                    .ToList();

                if (availableCourses == null || !availableCourses.Any())
                {
                    return NotFound("No available courses for this student.");
                }

                // Mapează cursurile rămase în DTO-uri
                var courseDTOs = availableCourses.Select(course => _courseMapper.MapToCourseDTO(course)).ToList();

                return Ok(courseDTOs);
            }
            else
            {
                return BadRequest("Invalid role.");
            }
        }

        [Authorize]
        [HttpGet("examrequests")]
        public async Task<IActionResult> GetAllExamRequests()
        {
            try
            {
                var examRequests = await _context.ExamRequests
                .Include(e => e.Group)
                    .ThenInclude(g => g.Specialization)
                    .ThenInclude(s => s.Faculty)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Professor)
                        .ThenInclude(p => p.Department)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Professor)
                        .ThenInclude(p => p.User)
                .Include(e => e.Assistant)
                    .ThenInclude(a => a.User)
                .Include(e => e.Assistant)
                    .ThenInclude(a => a.Department)
                .Include(e => e.Session)
                .ToListAsync();

                if (examRequests == null || !examRequests.Any())
                {
                    return NotFound("No exam requests found.");
                }

                return Ok(examRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetExamRequestsByGroupID/{groupId}")]
        public async Task<IActionResult> GetExamRequestsByGroupID(int groupId, string status = null)
        {
            try
            {
                var query = _context.ExamRequests
                    .Include(e => e.Group)
                        .ThenInclude(g => g.Specialization)
                        .ThenInclude(s => s.Faculty)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.Department)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.User)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.User)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.Department)
                    .Include(e => e.Session)
                    .Where(e => e.Group.GroupID == groupId);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(e => e.Status == status);
                }

                var examRequests = await query.ToListAsync();

                if (examRequests == null || !examRequests.Any())
                {
                    return NotFound($"No exam requests found for Group ID: {groupId}");
                }

                var examDTOs = examRequests.Select(exam => _courseMapper.MapToExamRequestDto(exam)).ToList();
                return Ok(examDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetExamRequestsByProfID/{profId}")]
        public async Task<IActionResult> GetExamRequestsByProfID(int profId, string status = null)
        {
            try
            {
                var query = _context.ExamRequests
                    .Include(e => e.Group)
                        .ThenInclude(g => g.Specialization)
                        .ThenInclude(s => s.Faculty)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.Department)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.User)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.User)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.Department)
                    .Include(e => e.Session)
                    .Where(e => e.Course.ProfessorID == profId);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(e => e.Status == status);
                }

                var examRequests = await query.ToListAsync();

                if (examRequests == null || !examRequests.Any())
                {
                    return NotFound($"No exam requests found for Group ID: {profId}");
                }

                var examDTOs = examRequests.Select(exam => _courseMapper.MapToExamRequestDto(exam)).ToList();
                return Ok(examDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAllRooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var requestedRooms = await _context.Rooms
     .OrderByDescending(r => r.RoomID) // Înlocuiețte `Id` cu o coloană relevantă pentru ordonare
     .Take(20)
     .ToListAsync();


                if (requestedRooms == null || !requestedRooms.Any())
                {
                    return NotFound("No requestedRooms found in the database.");
                }

                var roomDTOs = requestedRooms.Select(room => new
                {
                    room.RoomID,
                    room.Name,
                    room.Location,
                    room.Capacity,
                    room.Description,
                    DepartmentName = room.Department?.Name,
                    ExamRequestCount = requestedRooms.Count(rr => rr.RoomID == rr.RoomID)
                });

                return Ok(roomDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAssistentByCourse/{courseId}")]
        public async Task<IActionResult> GetAssistentByCourse(int courseId)
        {
            try
            {
                var professors = await _context.LabHolders
                 .Include(l => l.Professor)
                     .ThenInclude(p => p.User)
                 .Where(l => l.CourseID == courseId)
                 .Select(l => new ProfessorDTO
                 {
                     ProfID = l.Professor.ProfessorID,
                     LastName = l.Professor.User.LastName,
                     FirstName = l.Professor.User.FirstName
                 })
                 .Distinct() // Evită duplicatele
                 .ToListAsync();
                if (professors == null || !professors.Any())
                {
                    return NotFound($"No professors found for CourseID: {courseId}");
                }

                return Ok(professors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("UpdateExamStatus/{id}")]
        public async Task<IActionResult> UpdateExamStatus(int id ,[FromBody]UpdateExamRequestModel examModel)
        {

            if (string.IsNullOrEmpty(examModel.Status))
            {
                return BadRequest("Invalid status data.");
            }

            var existingExamRequest = await _context.ExamRequests.FindAsync(id);
            if (existingExamRequest == null)
            {
                return NotFound("Exam request not found.");
            }

            // Actualizarea câmpului `Status`
            existingExamRequest.Status = examModel.Status;

            // Salvarea modificărilor
            _context.ExamRequests.Update(existingExamRequest);
            await _context.SaveChangesAsync();

            return Ok(existingExamRequest);
        }

        [HttpGet("events/student/{userId}")]
        public async Task<IActionResult> GetStudentEvents(int userId)
        {
            try
            {
                // Get student's group
                var student = await _context.Students
                    .Include(s => s.Group)
                    .FirstOrDefaultAsync(s => s.UserID == userId);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                // Get exam requests for student's group
                var examRequests = await _context.ExamRequests
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.User)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.User)
                    .Include(e => e.Group)
                    .Where(e => e.GroupID == student.GroupID)
                    .ToListAsync();

                // Map to event format
                var events = examRequests.Select(exam => new
                {
                    id = exam.ExamRequestID.ToString(),
                    title = exam.Course.Title,
                    start = exam.TimeStart.HasValue ? exam.Date.Add(exam.TimeStart.Value) : exam.Date,
                    end = exam.TimeEnd.HasValue ? exam.Date.Add(exam.TimeEnd.Value) : exam.Date.AddHours(2), // Default 2-hour duration
                    isConfirmed = exam.Status == "Confirmed",
                    details = new
                    {
                        professor = new
                        {
                            firstName = exam.Course.Professor.User.FirstName,
                            lastName = exam.Course.Professor.User.LastName
                        },
                        assistant = exam.Assistant != null ? new
                        {
                            firstName = exam.Assistant.User.FirstName,
                            lastName = exam.Assistant.User.LastName
                        } : null,
                        group = exam.Group.Name,
                        type = exam.Type,
                        notes = exam.Details
                    }
                }).ToList();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("event/exam-request")]
        public async Task<IActionResult> CreateExamSuggestion([FromBody] ExamRequestDto examRequest)
        {
            try
            {
                // Validate input
                if (examRequest == null)
                {
                    return BadRequest("Invalid request data");
                }

                // Parse the date
                if (!DateTime.TryParse(examRequest.ExamDate, out DateTime parsedDate))
                {
                    return BadRequest("Invalid date format");
                }

                // Verify course exists
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.CourseID == examRequest.CourseId);
                if (course == null)
                {
                    return NotFound($"Course with ID {examRequest.CourseId} not found");
                }

                // Verify group exists
                var group = await _context.Groups
                    .FirstOrDefaultAsync(g => g.GroupID == examRequest.GroupId);
                if (group == null)
                {
                    return NotFound($"Group with ID {examRequest.GroupId} not found");
                }

                // Get current active session
                var activeSession = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.Status == "Active");
                if (activeSession == null)
                {
                    return BadRequest("No active exam session found");
                }

                // Create new exam request
                var newExamRequest = new ExamRequest
                {
                    CourseID = examRequest.CourseId,
                    GroupID = examRequest.GroupId,
                    SessionID = activeSession.SessionID,
                    Date = parsedDate,
                    Details = examRequest.Details,
                    Status = "Pending",
                    CreationDate = DateTime.UtcNow
                };

                _context.ExamRequests.Add(newExamRequest);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "Exam suggestion created successfully",
                    examRequestId = newExamRequest.ExamRequestID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}