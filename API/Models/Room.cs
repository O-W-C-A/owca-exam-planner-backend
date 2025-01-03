﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }
        public int? DepartmentID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int? Capacity { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        [ForeignKey("DepartmentID")]
        public virtual Department Department { get; set; }
    }


}
