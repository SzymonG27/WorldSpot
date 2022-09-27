/*using API.Data;
using API.Hubs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatHubController : ControllerBase
    {
        private readonly IHubContext<ChatHub> chatHub;
        private readonly IChatService chatService;
        private readonly IMessageService messageService;
        public ChatHubController(IHubContext<ChatHub> chatHub, IChatService chatService, IMessageService messageService)
        {
            this.chatHub = chatHub;
            this.chatService = chatService;
            this.messageService = messageService;
        }

        public async Task<IActionResult> Check(int id)
        {
            var chat = chatService.Get(id);
            if (chat == null)
            {

            }
        }


        [HttpGet]
        public IActionResult PushMessage(string senderName, string receiverGroup, string msg)
        {
            chatHub.Clients.Group(receiverGroup).SendAsync("ReceiveMessage", senderName, msg);
            return Ok();
        }
    }
}
*/