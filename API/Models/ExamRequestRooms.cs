using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class ExamRequestRooms
    {
        public int ExamRequestID { get; set; }
        public int RoomID { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("ExamRequestID")]
        public virtual ExamRequest ExamRequest { get; set; }

        [JsonIgnore]
        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; }
    }
}
