using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class SubSpecialization
    {
        [Key]
        public int SubSpecializationID { get; set; }
        public int SpecializationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("SpecializationID")]
        public virtual Specialization Specialization { get; set; }
    }

}
