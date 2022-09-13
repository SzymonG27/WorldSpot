using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TeamService : ITeamService
    {
        //inject
        private readonly ApplicationDbContext dbContext;
        public TeamService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<TeamModel>> Get()
        {
            return await dbContext.Teams.ToListAsync();
        }

        public async Task<TeamModel> Get(int id)
        {
            return await dbContext.Teams.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TeamModel> Create(TeamModel model)
        {
            dbContext.Teams.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(TeamModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var CommentToDelete = await dbContext.Teams.FindAsync(id);
            if (CommentToDelete != null)
            {
                dbContext.Teams.Remove(CommentToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
