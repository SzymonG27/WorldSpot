using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService teamService;
        public TeamController(ITeamService teamService)
        {
            this.teamService = teamService;
        }

        [HttpGet]
        public async Task<IEnumerable<TeamModel>> Get()
        {
            return await teamService.Get();
        }

        [HttpGet("{id}")]
        public async Task<TeamModel> Get(int id)
        {
            return await teamService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<TeamModel>> Post([FromBody] TeamModel model)
        {
            var newTeam = await teamService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newTeam.Id }, newTeam);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] TeamModel model)
        {
            await teamService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var teamToDelete = await teamService.Get(id);
            if (teamToDelete == null) return BadRequest();
            await teamService.Delete(teamToDelete.Id);
            return NoContent();
        }
    }
}
