using API.Models;
using API.Models.DTOmodels;

namespace API.Mapping
{
    public class CourseMapper
    {
        public CourseDTO MapToCourseDTO(Course course)
        {
            return new CourseDTO
            {
                Id = course.CourseID,
                Title = course.Title,
                NumeProfesor = course.Professor?.User?.FirstName,
                PrenumeProfesor = course.Professor?.User?.LastName,
                Status = course.Professor?.User?.Status,
            };
        }
        public ExamRequestDto MapToExamRequestDto(ExamRequest examRequest, List<ExamRequestRooms> examRequestRooms = null)
        {
            return new ExamRequestDto
            {
                Id = examRequest.ExamRequestID,
                CourseId = examRequest.Course.CourseID,
                GroupId = examRequest.GroupID,
                CourseName = examRequest.Course?.Title,
                FirstNameProf = examRequest.Course?.Professor?.User?.FirstName,
                LastNameProf = examRequest.Course?.Professor?.User?.LastName,
                ExamDate = examRequest.Date.ToString("yyyy-MM-dd"),
                TimeStart = examRequest.TimeStart,
                TimeEnd = examRequest.TimeEnd,
                Status = examRequest.Status,
                Details = examRequest.Details,
                Rooms = examRequestRooms?
                    .Where(err => err.ExamRequestID == examRequest.ExamRequestID)
                    .Select(err => new RoomDto
                    {
                        RoomID = err.Room.RoomID,
                        Name = err.Room.Name,
                        Location = err.Room.Location,
                        Description = err.Room.Description
                    }).ToList() ?? new List<RoomDto>() // Returnează o listă goală dacă examRequestRooms este null
                };
        }
    }
}
