using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        [JsonIgnore]

        [ForeignKey("GroupID")]

        public virtual Group Group { get; set; }
    }
}
