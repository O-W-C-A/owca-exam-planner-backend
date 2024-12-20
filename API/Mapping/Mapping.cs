﻿using API.Models;
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
        public ExamRequestDto MapToExamRequestDto(ExamRequest examRequest)
        {
            return new ExamRequestDto
            {
                Id = examRequest.ExamRequestID,
                CourseId = examRequest.Course.CourseID,
                CourseName = examRequest.Course?.Title,
                FirstNameProf = examRequest.Course?.Professor?.User?.FirstName,
                LastNameProf = examRequest.Course?.Professor?.User?.LastName,
                ExamDate = examRequest.Date,
                TimeStart = examRequest.TimeStart,
                Status = examRequest.Status,
                Details=examRequest.Details,
            };
        }
    }
}
