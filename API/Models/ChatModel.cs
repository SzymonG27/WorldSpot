using Server.Models;

namespace API.Models
{
    public class ChatModel
    {
        public int Id { get; set; }
        public ICollection<MessageModel> Messages { get; set; }
        public ICollection<AppUser> Users { get; set; }
    }
}
