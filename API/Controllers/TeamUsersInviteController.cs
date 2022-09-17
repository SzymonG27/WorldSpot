using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamUsersInviteController : ControllerBase
    {
        private readonly ITeamUsersInviteService teamUsersInviteService;
        public TeamUsersInviteController(ITeamUsersInviteService teamUsersInviteService)
        {
            this.teamUsersInviteService = teamUsersInviteService;
        }

        [HttpGet]
        public async Task<IEnumerable<TeamUsersInviteModel>> Get()
        {
            return await teamUsersInviteService.Get();
        }

        [HttpGet("{id}")]
        public async Task<TeamUsersInviteModel> Get(int id)
        {
            return await teamUsersInviteService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<TeamUsersInviteModel>> Post([FromBody] TeamUsersInviteModel model)
        {
            var newInvite = await teamUsersInviteService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newInvite.Id }, newInvite);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] TeamUsersInviteModel model)
        {
            await teamUsersInviteService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var inviteToDelete = await teamUsersInviteService.Get(id);
            if (inviteToDelete == null) return BadRequest();
            await teamUsersInviteService.Delete(inviteToDelete.Id);
            return NoContent();
        }
    }
}
