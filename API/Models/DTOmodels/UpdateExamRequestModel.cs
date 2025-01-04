namespace API.Models.DTOmodels
{
    public class UpdateExamRequestModel
    {
        public TimeSpan? TimeStart { get; set; }
        public TimeSpan? TimeEnd { get; set; }
        public int AssistantId { get; set; }
        public List<int> RoomsId { get; set; }
    }

}
