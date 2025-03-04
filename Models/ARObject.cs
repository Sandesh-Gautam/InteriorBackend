using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InteriorBackend.Models
{
    public class ARObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; } 

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [Required]
        public string ObjectType { get; set; }

        public string Position { get; set; }
        public string Rotation { get; set; }
        public string Scale { get; set; }
    }
}
