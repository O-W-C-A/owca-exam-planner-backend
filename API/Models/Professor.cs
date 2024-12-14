using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [JsonIgnore]
        [ForeignKey("DepartmentID")]
#nullable enable
        public virtual Department? Department { get; set; }
    }
}
