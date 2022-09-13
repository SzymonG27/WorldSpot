using API.Models;

namespace API.Services
{
    public interface IEventService
    {
        Task<List<EventModel>> Get();
        Task<EventModel> Get(int id);
        Task<EventModel> Create(EventModel model);
        Task Update(EventModel model);
        Task Delete(int id);
    }
}
