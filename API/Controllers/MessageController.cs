using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        public async Task<IEnumerable<MessageModel>> Get()
        {
            return await messageService.Get();
        }

        [HttpGet("{id}")]
        public async Task<MessageModel> Get(int id)
        {
            return await messageService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<MessageModel>> Post([FromBody] MessageModel model)
        {
            var newMessage = await messageService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newMessage.Id }, newMessage);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] MessageModel model)
        {
            await messageService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var messageToDelete = await messageService.Get(id);
            if (messageToDelete == null) return BadRequest();
            await messageService.Delete(messageToDelete.Id);
            return NoContent();
        }
    }
}
