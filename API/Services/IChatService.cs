using API.Models;

namespace API.Services
{
    public interface IChatService
    {
        Task<IEnumerable<ChatModel>> Get();
        Task<ChatModel> Get(int id);
        Task<ChatModel> Create(ChatModel model);
        Task Update(ChatModel model);
        Task Delete(int id);
    }
}
