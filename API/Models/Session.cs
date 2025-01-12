using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Session
    {
        [Key]
        public int SessionID { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Range(1, 2, ErrorMessage = "Semester must be either 1 or 2.")]
        public int Semester {  get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }

}
