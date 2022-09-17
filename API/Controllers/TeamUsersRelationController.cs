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
    public class TeamUsersRelationController : ControllerBase
    {
        private readonly ITeamUsersRelationService teamUsersRelationService;
        public TeamUsersRelationController(ITeamUsersRelationService teamUsersRelationService)
        {
            this.teamUsersRelationService = teamUsersRelationService;
        }

        [HttpGet]
        public async Task<IEnumerable<TeamUsersRelationModel>> Get()
        {
            return await teamUsersRelationService.Get();
        }

        [HttpGet("{id}")]
        public async Task<TeamUsersRelationModel> Get(int id)
        {
            return await teamUsersRelationService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<TeamUsersRelationModel>> Post([FromBody] TeamUsersRelationModel model)
        {
            var newRelation = await teamUsersRelationService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newRelation.Id }, newRelation);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] TeamUsersRelationModel model)
        {
            await teamUsersRelationService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var relationToDelete = await teamUsersRelationService.Get(id);
            if (relationToDelete == null) return BadRequest();
            await teamUsersRelationService.Delete(relationToDelete.Id);
            return NoContent();
        }
    }
}
