using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public async Task<ActionResult> DeleteTeam(int id)
        {
            var teamToDelete = await teamService.Get(id);
            if (teamToDelete == null) return BadRequest();
            await teamService.Delete(teamToDelete.Id);
            return NoContent();
        }

        [HttpGet("Check/{UserId}&{TeamName}")]
        public async Task<ActionResult> ValidateTeam(string userId, string teamName)
        {
            var isUserIdDuplicated = await teamService.RelationWithUser(userId);
            if (isUserIdDuplicated == true)
            {
                return Problem("Jeden użytkownik może mieć tylko 1 team", statusCode: (int)HttpStatusCode.Conflict);
            }
            var isTeamNameDuplicated = await teamService.IsTheSameName(teamName);
            if (isTeamNameDuplicated == true)
            {
                /*
                var response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Content = new StringContent("Nazwa teamu się powiela");
                var result = await Task.FromResult(response);
                return result;
                */
                return Problem("Nazwa teamu się powiela", statusCode: (int)HttpStatusCode.Conflict);
            }
            return Ok();
            
        }
    }
}
