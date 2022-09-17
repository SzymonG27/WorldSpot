using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class EventTeamRelationService : IEventTeamRelationService
    {
        private readonly ApplicationDbContext dbContext;
        public EventTeamRelationService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<EventTeamsRelationModel>> Get()
        {
            return await dbContext.EventTeamsRelations.ToListAsync();
        }

        public async Task<EventTeamsRelationModel> Get(int id)
        {
            return await dbContext.EventTeamsRelations.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<EventTeamsRelationModel> Create(EventTeamsRelationModel model)
        {
            dbContext.EventTeamsRelations.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(EventTeamsRelationModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var EventRelationToDelete = await dbContext.EventTeamsRelations.FindAsync(id);
            if (EventRelationToDelete != null)
            {
                dbContext.EventTeamsRelations.Remove(EventRelationToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
