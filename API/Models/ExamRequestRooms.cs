using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class ExamRequestRooms
    {
        public int ExamRequestID { get; set; }
        public int RoomID { get; set; }

        [ForeignKey("ExamRequestID")]
        public virtual ExamRequest ExamRequest { get; set; }

        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; }
    }
}
