using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FounderId { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public bool IsOpen { get; set; } //Czy ludzie mogą swobodnie dochodzić do grupy czy potrzebują zaproszenia
        public int Likes { get; set; }
    }
}
