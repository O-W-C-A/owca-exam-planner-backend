namespace API.Models
{
    public class CreateExamRequestDto
    {
        public int GroupID { get; set; }
        public int CourseID { get; set; }
        public List<int> RoomIDs { get; set; }
        public int AssistantID { get; set; }
        public int SessionID { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan Duration { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
    }
}
