using API.Models;

namespace API.Services
{
    public interface ITeamUsersInviteService
    {
        Task<IEnumerable<TeamUsersInviteModel>> Get();
        Task<TeamUsersInviteModel> Get(int id);
        Task<TeamUsersInviteModel> Create(TeamUsersInviteModel model);
        Task Update(TeamUsersInviteModel model);
        Task Delete(int id);

    }
}
