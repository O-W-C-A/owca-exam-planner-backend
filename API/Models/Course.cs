﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }
        public int ProfessorID { get; set; }
        public int SpecializationID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        [Range(1, 2, ErrorMessage = "Semester must be either 1 or 2.")]
        public int Semester { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("ProfessorID")]
        public virtual Professor Professor { get; set; }

        [JsonIgnore]
        [ForeignKey("SpecializationID")]
        public virtual Specialization Specialization { get; set; }
    }
}
