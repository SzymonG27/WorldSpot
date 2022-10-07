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

        public async Task<IEnumerable<TeamModel>> Get()
        {
            var teams = await dbContext.Teams.ToListAsync();
            return teams;
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

        public async Task<bool> RelationWithUser(string userId)
        {
            var isUserInRecord = await dbContext.Teams.FirstOrDefaultAsync(p => p.FounderId == userId);
            if (isUserInRecord != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsTheSameName(string name)
        {
            var isNameInDb = await dbContext.Teams.FirstOrDefaultAsync(p => p.Name == name);
            if (isNameInDb != null)
            {
                return true;
            }
            return false;
        }
    }
}
