using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class TeamUsersInvite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public int WhoApply { get; set; } //0 - Team zaprasza, 1 - Użytkownik chce dołączyć
    }
}
