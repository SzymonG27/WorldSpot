using API.Models;

namespace API.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageModel>> Get();
        Task<MessageModel> Get(int id);
        Task<MessageModel> Create(MessageModel model);
        Task Update(MessageModel model);
        Task Delete(int id);
    }
}
