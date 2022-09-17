using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventTeamsRelationController : ControllerBase
    {
        private readonly IEventTeamRelationService eventTeamRelationService;
        public EventTeamsRelationController(IEventTeamRelationService eventTeamRelationService)
        {
            this.eventTeamRelationService = eventTeamRelationService;
        }

        [HttpGet]
        public async Task<IEnumerable<EventTeamsRelationModel>> Get()
        {
            return await eventTeamRelationService.Get();
        }

        [HttpGet("{id}")]
        public async Task<EventTeamsRelationModel> Get(int id)
        {
            return await eventTeamRelationService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<EventTeamsRelationModel>> Post([FromBody] EventTeamsRelationModel model)
        {
            var newEventTeamRelation = await eventTeamRelationService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newEventTeamRelation.Id }, newEventTeamRelation);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] EventTeamsRelationModel model)
        {
            await eventTeamRelationService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var eventTeamsRelationToDelete = await eventTeamRelationService.Get(id);
            if (eventTeamsRelationToDelete == null) return BadRequest();
            await eventTeamRelationService.Delete(eventTeamsRelationToDelete.Id);
            return NoContent();
        }
    }
}
