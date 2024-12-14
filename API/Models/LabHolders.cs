using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class LabHolders
    {
        [Key]
        public int Id { get; set; }
        public int ProfessorID { get; set; }
        public int CourseID { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("ProfessorID")]
        public virtual Professor Professor { get; set; }

        [JsonIgnore]
        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }
    }

}
