using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class EventComment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; } //sortowanie
        public string Description { get; set; }
        public int Likes { get; set; } //sortowanie
    }
}
