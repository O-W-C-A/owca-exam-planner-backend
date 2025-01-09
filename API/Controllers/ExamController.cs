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
    [ApiController]
    [Authorize]  // Protect all endpoints in this controller by default
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
                    .Where(er => er.GroupID == student.GroupID)
                    .Select(er => er.CourseID)
                    .ToListAsync();

                var availableCourses = courses
                    .Where(course => !examRequests.Contains(course.CourseID))
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
                var examRequestRooms = await _context.ExamRequestRooms
                    .Include(err => err.Room)
                    .Where(err => examRequests.Select(e => e.ExamRequestID).Contains(err.ExamRequestID))
                    .ToListAsync();

                var examDTOs = examRequests
                .Select(exam => _courseMapper.MapToExamRequestDto(exam, examRequestRooms))
                .ToList();

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
                var examRequestRooms = await _context.ExamRequestRooms
                   .Include(err => err.Room)
                   .Where(err => examRequests.Select(e => e.ExamRequestID).Contains(err.ExamRequestID))
                   .ToListAsync();

                var examDTOs = examRequests
                .Select(exam => _courseMapper.MapToExamRequestDto(exam, examRequestRooms))
                .ToList();
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

        [HttpPut("event/exam-request/{examId}/approve")]
        public async Task<IActionResult> UpdateExamStatus(int examId, [FromBody]UpdateExamRequestModel examModel)
        {
            var existingExamRequest = await _context.ExamRequests.FindAsync(examId);
            if (existingExamRequest == null)
            {
                return NotFound("Exam request not found.");
            }

            // Actualizarea câmpului `Status`
            existingExamRequest.Status = ExamRequestStatusEnum.Approved;
            existingExamRequest.TimeStart = examModel.TimeStart;
            existingExamRequest.TimeEnd = examModel.TimeEnd;

            if (examModel.RoomsId != null && examModel.RoomsId.Any())
            {
                // Ștergerea vechilor camere asociate
                var existingRooms = await _context.ExamRequestRooms
                                                  .Where(er => er.ExamRequestID == examId)
                                                  .ToListAsync();

                _context.ExamRequestRooms.RemoveRange(existingRooms);

                // Crearea noilor asocieri
                var newRooms = examModel.RoomsId.Select(roomId => new ExamRequestRooms
                {
                    ExamRequestID = examId,
                    RoomID = roomId,
                    CreationDate = DateTime.Now
                }).ToList();

                // Adăugarea noilor camere
                _context.ExamRequestRooms.AddRange(newRooms);
            }

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


                var examRequestRooms = await _context.ExamRequestRooms
                    .Include(err => err.Room)
                    .Where(err => examRequests.Select(e => e.ExamRequestID).Contains(err.ExamRequestID))
                    .ToListAsync();

                // Group rooms by ExamRequestID
                var roomsByExamRequestId = examRequestRooms
                    .GroupBy(err => err.ExamRequestID)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(r => new RoomDto
                        {
                            RoomID = r.Room.RoomID,
                            Name = r.Room.Name,
                            Location = r.Room.Location,
                            Capacity = 0,
                            Description = r.Room.Description
                        }).ToList()
                    );

                var events = examRequests.Select(exam => MapToEventFormat(exam, roomsByExamRequestId)).ToList();

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
                    Status = ExamRequestStatusEnum.Pending,
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

        [HttpGet("event/exam-request/professor/{userId}/course/{courseId}")]
        public async Task<IActionResult> GetExamRequestsByProfessorAndCourse(int userId, int courseId)
        {
            try
            {
                var professor = await _context.Professors
                    .FirstOrDefaultAsync(p => p.UserID == userId);

                if (professor == null)
                {
                    return NotFound($"No professor found for user ID {userId}");
                }

                var examRequests = await _context.ExamRequests
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.User)
                    .Include(e => e.Group)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.User)
                    .Where(e => e.Course.ProfessorID == professor.ProfessorID && e.CourseID == courseId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();

                var examRequestRooms = await _context.ExamRequestRooms
                    .Include(err => err.Room)
                    .Where(err => examRequests.Select(e => e.ExamRequestID).Contains(err.ExamRequestID))
                    .ToListAsync();

                // Group rooms by ExamRequestID
                var roomsByExamRequestId = examRequestRooms
                    .GroupBy(err => err.ExamRequestID)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(r => new RoomDto
                        {
                            RoomID = r.Room.RoomID,
                            Name = r.Room.Name,
                            Location = r.Room.Location,
                            Capacity = 0,
                            Description = r.Room.Description
                        }).ToList()
                    );

                var events = examRequests.Select(exam => MapToEventFormat(exam, roomsByExamRequestId)).ToList();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet("event/exam-request/professor/{userId}")]
        public async Task<IActionResult> GetAllExamRequestsByProfessor(int userId)
        {
            try
            {
                var professor = await _context.Professors
                    .FirstOrDefaultAsync(p => p.UserID == userId);

                if (professor == null)
                {
                    return NotFound($"No professor found for user ID {userId}");
                }

                var examRequests = await _context.ExamRequests
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.User)
                    .Include(e => e.Group)
                    .Include(e => e.Assistant)
                        .ThenInclude(a => a.User)
                    .Where(e => e.Course.ProfessorID == professor.ProfessorID)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();

                var examRequestRooms = await _context.ExamRequestRooms
                    .Include(err => err.Room)
                    .Where(err => examRequests.Select(e => e.ExamRequestID).Contains(err.ExamRequestID))
                    .ToListAsync();

                // Group rooms by ExamRequestID
                var roomsByExamRequestId = examRequestRooms
                    .GroupBy(err => err.ExamRequestID)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(r => new RoomDto
                        {
                            RoomID = r.Room.RoomID,
                            Name = r.Room.Name,
                            Location = r.Room.Location,
                            Capacity = 0,
                            Description = r.Room.Description
                        }).ToList()
                    );

                var events = examRequests.Select(exam => MapToEventFormat(exam, roomsByExamRequestId)).ToList();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetLabHoldersByCourseID/{courseId}")]
        public async Task<IActionResult> GetLabHoldersByCourseID(int courseId)
        {
            try
            {
                // Fetch all lab holders for the given course ID
                var labHolders = await _context.LabHolders
                    .Where(p => p.CourseID == courseId)
                    .ToListAsync();

                if (!labHolders.Any())
                {
                    return NotFound($"No labHolders found for course ID {courseId}");
                }

                // Fetch associated professors
                var professors = await _context.Professors
                    .Include(e => e.User)
                    .Where(p => labHolders.Select(l => l.ProfessorID).Contains(p.ProfessorID))
                    .Select(p => new
                    {
                        ProfessorId = p.ProfessorID,
                        UserId = p.UserID,
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName
                    })
                    .ToListAsync();

                if (!professors.Any())
                {
                    return NotFound($"No professors found for lab holders in course ID {courseId}");
                }

                return Ok(professors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("event/exam-request/{examId}/reject")]
        public async Task<IActionResult> RejectExamRequest(int examId, [FromBody] RejectRequestDto request)
        {
            try
            {
                var examRequest = await _context.ExamRequests
                    .Include(e => e.Course)
                    .FirstOrDefaultAsync(e => e.ExamRequestID == examId);

                if (examRequest == null)
                {
                    return NotFound($"Exam request with ID {examId} not found");
                }

                // Update the exam request
                examRequest.Status = ExamRequestStatusEnum.Rejected;
                examRequest.Details = request.Reason;  // Store rejection reason in details

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "Exam request rejected successfully",
                    examRequestId = examId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Common event mapping function to maintain consistency
        private object MapToEventFormat(ExamRequest exam, Dictionary<int, List<RoomDto>> roomsByExamRequestId)
        {
            roomsByExamRequestId.TryGetValue(exam.ExamRequestID, out var rooms);

            return new
            {
                id = exam.ExamRequestID.ToString(),
                title = exam.Course.Title,
                courseId = exam.Course.CourseID,
                date = exam.Date.ToString("yyyy-MM-dd"),
                start = exam.TimeStart,
                end = exam.TimeEnd,
                status = exam.Status,
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
                    notes = exam.Details,
                    rooms = rooms
                }
            };
        }
    }
}