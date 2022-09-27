using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext dbContext;
        public ChatService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<ChatModel>> Get()
        {
            return await dbContext.Chats.ToListAsync();
        }

        public async Task<ChatModel> Get(int id)
        {
            return await dbContext.Chats
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ChatModel> Create(ChatModel model)
        {
            dbContext.Chats.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(ChatModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var chatToDelete = await dbContext.Chats.FindAsync(id);
            if (chatToDelete != null)
            {
                dbContext.Chats.Remove(chatToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
