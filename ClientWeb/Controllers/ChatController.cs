using ClientWeb.Models;
using ClientWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientWeb.Controllers
{
    public class ChatController : Controller
    {
        private HubConnection hubConnection;
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        public List<string> messagesList = new List<string>();

        public ChatController(ITokenService tokenService, IConfiguration configuration)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
        }

        public IActionResult Index(string id, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            hubConnection = new HubConnectionBuilder()
                .WithUrl(configuration["apiUrl"] + "/chathub")
                .Build();

            //TODO
            //hubConnection.InvokeAsync<ChatMessageModel>("ReceiveMessage", new ChatMessageModel() { Group = id, Message = });

            return View();
        }
    }
}
