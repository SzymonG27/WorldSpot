using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWeb.Models
{
    public class TeamUsersRelationModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TeamId { get; set; }
        public string Rank { get; set; } //Ranga w teamie, użytkownik od danej rangi będzie mógł zapraszać i akceptować ludzi oraz organizować wydarzenia
    }
}
