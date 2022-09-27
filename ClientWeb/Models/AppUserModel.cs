using Microsoft.AspNetCore.Identity;

namespace ClientWeb.Models
{
    public class AppUserModel : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
