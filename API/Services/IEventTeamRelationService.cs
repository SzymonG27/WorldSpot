using API.Models;

namespace API.Services
{
    public interface IEventTeamRelationService
    {
        Task<IEnumerable<EventTeamsRelationModel>> Get();
        Task<EventTeamsRelationModel> Get(int id);
        Task<EventTeamsRelationModel> Create(EventTeamsRelationModel model);
        Task Update(EventTeamsRelationModel model);
        Task Delete(int id);
    }
}
