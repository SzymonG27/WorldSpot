using API.Models;

namespace API.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamModel>> Get();
        Task<TeamModel> Get(int id);
        Task<TeamModel> Create(TeamModel model);
        Task Update(TeamModel model);
        Task Delete(int id);
        Task<bool> RelationWithUser(string userId);
        Task<bool> IsTheSameName(string name);
    }
}
