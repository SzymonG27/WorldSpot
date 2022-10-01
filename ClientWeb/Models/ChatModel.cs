
namespace ClientWeb.Models
{
    public class ChatModel
    {
        public ChatModel()
        {
            Messages = new List<MessageModel>();
            Users = new List<AppUserModel>();
        }
        public int Id { get; set; }
        public int TeamId { get; set; }
        public ICollection<MessageModel> Messages { get; set; }
        public ICollection<AppUserModel> Users { get; set; }
    }
}
