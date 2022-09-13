using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext dbContext;
        public CommentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<CommentModel>> Get()
        {
            return await dbContext.Comments.ToListAsync();
        }

        public async Task<CommentModel> Get(int id)
        {
            return await dbContext.Comments.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<CommentModel> Create(CommentModel model)
        {
            dbContext.Comments.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(CommentModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var CommentToDelete = await dbContext.Comments.FindAsync(id);
            if (CommentToDelete != null)
            {
                dbContext.Comments.Remove(CommentToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
