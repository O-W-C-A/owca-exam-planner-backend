using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public int FacultyID { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public int UniversityID { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("FacultyID")]
        public virtual Faculty Faculty { get; set; }
    }
}
