namespace API.Models.DTOmodels
{
    public class UpdateExamRequestModel
    {
        public string Status { get; set; }
        public TimeSpan? TimeStart { get; set; }
        public TimeSpan? TimeEnd { get; set; }
        public DateTime ExamDate { get; set; }
        public int AssistentId { get; set; }
        public List<int> RoomsId { get; set; }
    }

}
