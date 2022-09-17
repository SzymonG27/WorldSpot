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
    public class EventController : ControllerBase
    {
        private readonly IEventService eventService;
        public EventController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        [HttpGet]
        public async Task<IEnumerable<EventModel>> Get()
        {
            return await eventService.Get();
        }

        [HttpGet("{id}")]
        public async Task<EventModel> Get(int id)
        {
            return await eventService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<EventModel>> Post([FromBody] EventModel model)
        {
            var newEvent = await eventService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newEvent.Id }, newEvent);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] EventModel model)
        {
            await eventService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var eventToDelete = await eventService.Get(id);
            if (eventToDelete == null) return BadRequest();
            await eventService.Delete(eventToDelete.Id);
            return NoContent();
        }
    }
}
