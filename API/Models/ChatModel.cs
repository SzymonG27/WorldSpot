using Server.Models;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace API.Models
{
    public class ChatModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }

        public ICollection<MessageModel> Messages { get; set; }

        public ICollection<AppUser> Users { get; set; }
    }
}
