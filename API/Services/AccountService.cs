using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System.Security.Claims;

namespace API.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppIdentityDbContext dbContext; //odwołanie do projektu Server
        private readonly UserManager<AppUser> userManager;
        public AccountService(AppIdentityDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<AppUser>> Get()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<AppUser> Get(string id)
        {
            return await dbContext.Users.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<AppUser> GetUserFromNick(string nickName)
        {
            return await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == nickName);
        }

        public async Task<bool> CheckMail(string mail) //true - mail istnieje, false nieistnieje
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == mail);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsMailConfirmed(string mail)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == mail);
            if (user.EmailConfirmed == true)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Register(AppUser user, string password)
        {
            var res = await userManager.CreateAsync(user, password);
            if (res.Succeeded)
            {
                return true;
            }
            return false;
        }
        
        public async Task<bool> AddClaims(AppUser user)
        {
            var res = await userManager.AddClaimsAsync(user, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim(JwtClaimTypes.GivenName, user.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, user.LastName),
                    new Claim(JwtClaimTypes.NickName, user.UserName),
                    new Claim("Photo", ""), //Puste, będzie dodane dodawanie zdjęć do api w późniejszych wypchnięciach
                    new Claim("DateOfJoin", user.DateJoined.ToString())

                }
            );

            if (res.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task Delete(string id)
        {
            var userToDelete = await dbContext.Users.FindAsync(id);
            if (userToDelete != null)
            {
                dbContext.Users.Remove(userToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
