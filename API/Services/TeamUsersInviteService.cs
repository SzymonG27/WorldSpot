using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TeamUsersInviteService : ITeamUsersInviteService
    {
        private readonly ApplicationDbContext dbContext;
        public TeamUsersInviteService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<TeamUsersInviteModel>> Get()
        {
            return await dbContext.TeamUsersInvites.ToListAsync();
        }

        public async Task<TeamUsersInviteModel> Get(int id)
        {
            return await dbContext.TeamUsersInvites.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TeamUsersInviteModel> Create(TeamUsersInviteModel model)
        {
            dbContext.TeamUsersInvites.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(TeamUsersInviteModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var UserInviteToDelete = await dbContext.TeamUsersInvites.FindAsync(id);
            if (UserInviteToDelete != null)
            {
                dbContext.TeamUsersInvites.Remove(UserInviteToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
