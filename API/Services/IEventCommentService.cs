using API.Models;

namespace API.Services
{
    public interface IEventCommentService
    {
        Task<List<EventCommentModel>> Get();
        Task<EventCommentModel> Get(int id);
        Task<EventCommentModel> Create(EventCommentModel model);
        Task Update(EventCommentModel model);
        Task Delete(int id);
    }
}
