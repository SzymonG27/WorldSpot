using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        public string Photo { get; set; }

        [Required]
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}
