﻿using API.Models.DTOmodels;
using API.Models;

namespace API.Mapping
{
    public class CourseMapper
    {
        public CourseDTO MapToCourseDTO(Course course)
        {
            return new CourseDTO
            {
                Title = course.Title,
                NumeProfesor = course.Professor?.User?.FirstName,
                PrenumeProfesor = course.Professor?.User?.LastName,
                Status = course.Professor?.User?.Status
            };
        }
    }
}
