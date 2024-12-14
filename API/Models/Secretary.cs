using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Secretary
    {
        [Key]
        public int SecretaryID { get; set; }
        public int UserID { get; set; }
        public int SpecializationID { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [JsonIgnore]
        [ForeignKey("SpecializationID")]
        public virtual Specialization Specialization { get; set; }
    }
}
