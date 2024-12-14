using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class ExamRequest
    {
        [Key]
        public int ExamRequestID { get; set; }
        public int GroupID { get; set; }
        public int CourseID { get; set; }
        public int AssistantID { get; set; }
        public int SessionID { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        [JsonIgnore]
        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }

        [JsonIgnore]
        [ForeignKey("AssistantID")]
        public virtual Professor Assistant { get; set; }

        [JsonIgnore]
        [ForeignKey("SessionID")]
        public virtual Session Session { get; set; }
    }


}
