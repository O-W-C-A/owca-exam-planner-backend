using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public bool IsLeader { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("GroupID")]

        public virtual Group Group { get; set; }
    }
}
