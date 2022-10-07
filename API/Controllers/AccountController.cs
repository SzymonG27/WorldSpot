using API.Models;
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

        [HttpGet("GetFromNick/{nickName}")]
        public async Task<ActionResult<AppUser>> GetFromNick(string nickName)
        {
            var user = await accountService.GetUserFromNick(nickName);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        [HttpGet("CheckMail/{mail}")]
        public async Task<ActionResult> CheckMail(string mail)
        {
            var result = await accountService.CheckMail(mail);
            if (result == true)
            {
                return Ok();
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<AppUser>> RegisterUser(UserPasswordModel userPassword)
        {
            var registerUser = await accountService.Register(userPassword.AppUserModel, userPassword.Password);
            if (registerUser == false) return BadRequest();
            var registerClaims = await accountService.AddClaims(userPassword.AppUserModel);
            if (registerClaims == true) return Ok();
            await accountService.Delete(userPassword.AppUserModel.Id);  //Jeżeli będzie problem z dodaniem claimów, usunie użytkownika
            return BadRequest();
        }
    }
}
