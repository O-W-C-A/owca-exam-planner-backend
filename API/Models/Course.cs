using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }
        public int ProfessorID { get; set; }
        public int SpecializationID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("ProfessorID")]
        public virtual Professor Professor { get; set; }

        [ForeignKey("SpecializationID")]
        public virtual Specialization Specialization { get; set; }

        [ForeignKey("SubSpecializationID")]
        public virtual SubSpecialization SubSpecialization { get; set; }
    }
}
