using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InteriorBackend.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        public string? EmailConfirmationCode { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
