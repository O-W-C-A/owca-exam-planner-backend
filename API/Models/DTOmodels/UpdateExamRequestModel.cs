namespace API.Models.DTOmodels
{
    public class UpdateExamRequestModel
    {
        public string Status { get; set; }
        public int AssistentId { get; set; }
        public List<int> RoomsId { get; set; }
    }

}
