using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Group
    {
        [Key]
        public int GroupID { get; set; }
        public int SpecializationID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("SpecializationID")]
        public virtual Specialization Specialization { get; set; }
    }
}
