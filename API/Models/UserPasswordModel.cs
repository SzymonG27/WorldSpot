using Server.Models;

namespace API.Models
{
    public class UserPasswordModel
    {
        public AppUser AppUserModel { get; set; }
        public string Password { get; set; }
    }
}
