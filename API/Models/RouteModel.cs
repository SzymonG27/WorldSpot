using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class RouteModel //stricte do wyścigów na aplikacje mobilne, będą się robić punkty na mapie
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public float StartX { get; set; }
        public float StartY { get; set; }
        public float EndX { get; set; }
        public float EndY { get; set; }
    }
}
