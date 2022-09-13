using API.Models;

namespace API.Services
{
    public interface IEventTeamRelationService
    {
        Task<List<EventTeamsRelationModel>> Get();
        Task<EventTeamsRelationModel> Get(int id);
        Task<EventTeamsRelationModel> Create(EventTeamsRelationModel model);
        Task Update(EventTeamsRelationModel model);
        Task Delete(int id);
    }
}
