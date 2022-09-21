using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string TeamId { get; set; } //Może być parę organizatorów, będzie się zapisywać po przecinku a potem splitować dlatego string. Nowy input dodawany poprzez +
        public DateTime CreationDate { get; set; } //Wydarzenie będzie przechodzić do archiwum
        public string Title { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Direction { get; set; }
        public string Photo { get; set; } //Zdjęcie będzie się usuwać po przejściu do archiwum żeby nie obciążać serwera
        public int Likes { get; set; }    //Sortowanie ciekawych wydarzeń dzięki polubieniu
    }
}
