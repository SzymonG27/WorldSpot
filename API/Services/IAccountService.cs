using API.Models;
using Server.Models;

namespace API.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<AppUser>> Get();
        Task<AppUser> Get(string id);
    }
}
