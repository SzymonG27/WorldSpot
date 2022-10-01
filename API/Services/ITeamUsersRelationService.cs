using API.Models;

namespace API.Services
{
    public interface ITeamUsersRelationService
    {
        Task<IEnumerable<TeamUsersRelationModel>> Get();
        Task<TeamUsersRelationModel> Get(int id);
        Task<TeamUsersRelationModel> GetRelation(int teamId, string userId);
        Task<TeamUsersRelationModel> Create(TeamUsersRelationModel model);
        Task Update(TeamUsersRelationModel model);
        Task Delete(int id);
    }
}
