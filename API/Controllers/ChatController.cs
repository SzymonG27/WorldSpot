using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet]
        public async Task<IEnumerable<ChatModel>> Get()
        {
            return await chatService.Get();
        }

        [HttpGet("{id}")]
        public async Task<ChatModel> Get(int id)
        {
            return await chatService.Get(id);
        }

        [HttpGet("GetFromTeam/{teamId}")]
        public async Task<ActionResult<ChatModel>> GetFromTeam(int teamId)
        {
            var isExists = await chatService.GetFromTeam(teamId);
            if (isExists == null)
            {
                return NotFound();
            }
            return isExists;
        }

        [HttpPost]
        public async Task<ActionResult<ChatModel>> Post([FromBody] ChatModel model)
        {
            var newChat = await chatService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newChat.Id }, newChat);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] ChatModel model)
        {
            await chatService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var chatToDelete = await chatService.Get(id);
            if (chatToDelete == null) return BadRequest();
            await chatService.Delete(chatToDelete.Id);
            return NoContent();
        }
    }
}
