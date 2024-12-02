using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Professor
    {
        [Key]
        public int ProfessorID { get; set; }
        public int UserID { get; set; }
        public int? DepartmentID { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }
    }
}
