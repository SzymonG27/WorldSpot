using API.Models;
using Server.Models;

namespace API.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AppUser>> Get();
        Task<AppUser> Get(string id);
        Task<AppUser> GetUserFromNick(string nickName);
        Task<bool> CheckMail(string mail);
        Task<bool> IsMailConfirmed(string mail);
        Task<bool> Register(AppUser user, string password);
        Task<bool> AddClaims(AppUser user);
        Task Delete(string id);
    }
}
