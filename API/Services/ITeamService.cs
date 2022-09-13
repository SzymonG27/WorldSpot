using API.Models;

namespace API.Services
{
    public interface ITeamService
    {
        Task<List<TeamModel>> Get();
        Task<TeamModel> Get(int id);
        Task<TeamModel> Create(TeamModel model);
        Task Update(TeamModel model);
        Task Delete(int id);
    }
}
