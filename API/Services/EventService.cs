using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext dbContext;
        public EventService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<EventModel>> Get()
        {
            return await dbContext.Events.ToListAsync();
        }

        public async Task<EventModel> Get(int id)
        {
            return await dbContext.Events.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<EventModel> Create(EventModel model)
        {
            dbContext.Events.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(EventModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var EventToDelete = await dbContext.Events.FindAsync(id);
            if (EventToDelete != null)
            {
                dbContext.Events.Remove(EventToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
