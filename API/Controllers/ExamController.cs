﻿using API.Data;
using API.Enum;
using API.Mapping;
using API.Models;
using API.Models.DTOmodels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .Include(e => e.ExamRequestRooms)
                    .ThenInclude(er => er.Room)
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
                    .Include(e => e.ExamRequestRooms)
                        .ThenInclude(er => er.Room)
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
                    .Include(e => e.ExamRequestRooms)
                        .ThenInclude(er => er.Room)
                    .Where(e => e.Course.ProfID == profId);

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
                var rooms = await _context.Rooms
                    .Include(r => r.Department)
                    .Include(r => r.ExamRequestRooms)
                        .ThenInclude(er => er.ExamRequest)
                         .Take(10)
                    .ToListAsync();

                if (rooms == null || !rooms.Any())
                {
                    return NotFound("No rooms found in the database.");
                }

                var roomDTOs = rooms.Select(room => new
                {
                    room.RoomID,
                    room.Name,
                    room.Location,
                    room.Capacity,
                    room.Description,
                    room.CreationDate,
                    DepartmentName = room.Department?.Name, 
                    ExamRequestCount = room.ExamRequestRooms?.Count ?? 0
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
                     ProfID = l.Professor.ProfID,
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
        public async Task<IActionResult> UpdateExamStatus(int id, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Invalid status data.");
            }

            var existingExamRequest = await _context.ExamRequests.FindAsync(id);
            if (existingExamRequest == null)
            {
                return NotFound("Exam request not found.");
            }

            // Actualizarea câmpului `Status`
            existingExamRequest.Status = status;

            // Salvarea modificărilor
            _context.ExamRequests.Update(existingExamRequest);
            await _context.SaveChangesAsync();

            return Ok(existingExamRequest);
        }

    }
}