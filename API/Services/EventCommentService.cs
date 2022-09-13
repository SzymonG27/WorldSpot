using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class EventCommentService : IEventCommentService
    {
        private readonly ApplicationDbContext dbContext;
        public EventCommentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<EventCommentModel>> Get()
        {
            return await dbContext.EventsComments.ToListAsync();
        }

        public async Task<EventCommentModel> Get(int id)
        {
            return await dbContext.EventsComments.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<EventCommentModel> Create(EventCommentModel model)
        {
            dbContext.EventsComments.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(EventCommentModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var EventComToDelete = await dbContext.EventsComments.FindAsync(id);
            if (EventComToDelete != null)
            {
                dbContext.EventsComments.Remove(EventComToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
