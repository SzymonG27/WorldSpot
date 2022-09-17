using API.Models;

namespace API.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentModel>> Get();
        Task<CommentModel> Get(int id);
        Task<CommentModel> Create(CommentModel model);
        Task Update(CommentModel model);
        Task Delete(int id);
    }
}
