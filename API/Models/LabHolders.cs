using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class LabHolders
    {
        public int ProfessorID { get; set; }
        public int CourseID { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("ProfessorID")]
        public virtual Professor Professor { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }
    }

}
