using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Session
    {
        [Key]
        public int SessionID { get; set; }
        public bool Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }

}
