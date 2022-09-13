using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class EventTeamsRelation
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int EventId { get; set; }
        public bool IsAccepted { get; set; } //Po przeniesieniu danego eventu do archiwum będzie się automatycznie relacja usuwać z bazy
        //Jeżeli team zaakceptuje wyśle się powiadomienie do użytkowników należących do niego -> przejdź do api kontroler: ...
    }
}
