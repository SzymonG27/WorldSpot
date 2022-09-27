using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext dbContext;
        public MessageService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<MessageModel>> Get()
        {
            return await dbContext.Messages.ToListAsync();
        }

        public async Task<MessageModel> Get(int id)
        {
            return await dbContext.Messages.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<MessageModel> Create(MessageModel model)
        {
            dbContext.Messages.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(MessageModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var messgeToDelete = await dbContext.Messages.FindAsync(id);
            if (messgeToDelete != null)
            {
                dbContext.Messages.Remove(messgeToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
