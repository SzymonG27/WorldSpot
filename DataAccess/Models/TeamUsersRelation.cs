using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class TeamUsersRelation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public string Rank { get; set; } //Ranga w teamie, użytkownik od danej rangi będzie mógł zapraszać i akceptować ludzi oraz organizować wydarzenia
    }
}
