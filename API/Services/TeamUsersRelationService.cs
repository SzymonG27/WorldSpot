using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TeamUsersRelationService : ITeamUsersRelationService
    {
        private readonly ApplicationDbContext dbContext;
        public TeamUsersRelationService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<TeamUsersRelationModel>> Get()
        {
            return await dbContext.TeamUsersRelations.ToListAsync();
        }

        public async Task<TeamUsersRelationModel> Get(int id)
        {
            return await dbContext.TeamUsersRelations.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TeamUsersRelationModel> Create(TeamUsersRelationModel model)
        {
            dbContext.TeamUsersRelations.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(TeamUsersRelationModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var TeamUsrRelationToDelete = await dbContext.TeamUsersRelations.FindAsync(id);
            if (TeamUsrRelationToDelete != null)
            {
                dbContext.TeamUsersRelations.Remove(TeamUsrRelationToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
