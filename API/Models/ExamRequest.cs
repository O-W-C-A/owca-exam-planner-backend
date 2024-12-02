using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public TimeSpan Duration { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }

        [ForeignKey("AssistantID")]
        public virtual Professor Assistant { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session Session { get; set; }
    }


}
