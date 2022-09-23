using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace API.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppIdentityDbContext dbContext; //odwołanie do projektu Server
        public AccountService(AppIdentityDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<AppUser>> Get()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<AppUser> Get(string id)
        {
            return await dbContext.Users.SingleOrDefaultAsync(t => t.Id == id);
        }
    }
}
