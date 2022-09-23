using API.Services;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public async Task<IEnumerable<AppUser>> Get()
        {
            return await accountService.Get();
        }

        [HttpGet("{id}")]
        public async Task<AppUser> Get(string id)
        {
            return await accountService.Get(id);
        }
    }
}
