using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWeb.Models
{
    public class TeamModel
    {
        public int Id { get; set; }
        [Display(Name = "Nazwa teamu")]
        public string Name { get; set; }
        public string FounderId { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public bool IsOpen { get; set; } //Czy ludzie mogą swobodnie dochodzić do grupy czy potrzebują zaproszenia
        public int Likes { get; set; }
    }
}
