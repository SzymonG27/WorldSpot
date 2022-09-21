using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class TeamUsersInviteModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TeamId { get; set; }
        public int WhoApply { get; set; } //0 - Team zaprasza, 1 - Użytkownik chce dołączyć
    }
}
