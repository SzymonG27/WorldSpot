using API.Models;

namespace API.Services
{
    public interface ITeamUsersRelationService
    {
        Task<List<TeamUsersRelationModel>> Get();
        Task<TeamUsersRelationModel> Get(int id);
        Task<TeamUsersRelationModel> Create(TeamUsersRelationModel model);
        Task Update(TeamUsersRelationModel model);
        Task Delete(int id);
    }
}
