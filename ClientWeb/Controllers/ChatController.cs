using ClientWeb.Hubs;
using ClientWeb.Models;
using ClientWeb.Services;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ClientWeb.Controllers
{
    [Route("[controller]")]
    public class ChatController : Controller
    {
        //private HubConnection hubConnection;
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        private readonly IHubContext<ChatHub> hubContext;
        public List<string> messagesList = new List<string>();

        public ChatController(ITokenService tokenService, IConfiguration configuration, IHubContext<ChatHub> hubContext)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id, string returnUrl) //tutaj id to id teamu, do którego chat jest przypisany
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Fail"] = "Najpierw musisz być zalogowany!";
                return RedirectToAction("Login", "Account");
            }
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);


                //Sprawdzamy czy użytkownik jest w teamie
                var checkRelation = await client.GetAsync("/api/TeamUsersRelation/Check/" + id + "&" + userId);
                if (!checkRelation.IsSuccessStatusCode || checkRelation.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Fail"] = "Nie należysz do tego teamu";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }


                //Sprawdzamy czy w teamie istnieje taki chat (tworzy się przy tworzeniu teamu -> TeamController)
                var isChatExists = await client.GetAsync("/api/Chat/GetFromTeam/" + id);
                if (!isChatExists.IsSuccessStatusCode || isChatExists.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Fail"] = "Nie ma czatu o podanym id";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                var chatContent = JsonConvert.DeserializeObject<ChatModel>(await isChatExists.Content.ReadAsStringAsync());

                return View(chatContent);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, int teamId, string message)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Fail"] = "Najpierw musisz być zalogowany!";
                return RedirectToAction("Login", "Account");
            }
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                

                var getUser = await client.GetAsync("api/Account/" + userId);
                var user = JsonConvert.DeserializeObject<AppUserModel>(await getUser.Content.ReadAsStringAsync());
                var messModel = new MessageModel
                {
                    ChatModelId = chatId,
                    Message = message,
                    UserId = userId,
                    UserName = user.UserName,
                    CreatedDate = DateTime.Now,
                };

                var messJson = JsonConvert.SerializeObject(messModel);
                var buffer = Encoding.UTF8.GetBytes(messJson);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync("/api/Message", byteContent);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem i wiadomość nie została wysłana";
                }
            }
            return RedirectToAction("Index", new { id = teamId });
        }

        //-----------------SignalR Results
        [HttpPost("[action]/{connectionId}/{chatId}")]
        public async Task<IActionResult> JoinChat(string connectionId, int chatId)
        {
            await hubContext.Groups.AddToGroupAsync(connectionId, chatId.ToString());
            return Ok();
        }

        [HttpPost("[action]/{connectionId}/{chatId}")]
        public async Task<IActionResult> LeaveChat(string connectionId, int chatId)
        {
            await hubContext.Groups.RemoveFromGroupAsync(connectionId, chatId.ToString());
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string message, int chatId, int teamId) 
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(configuration["apiUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
            client.SetBearerToken(tokenResponse.AccessToken);

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userResponse = await client.GetAsync("api/Account/" + userId);
            if (!userResponse.IsSuccessStatusCode) //Nie powinno się wydarzyć
            {
                TempData["Fail"] = "Nie znaleziono takiego użytkownika w bazie! Nie wysłano wiadomości";
                return RedirectToAction("Index", new { id = chatId });
            }
            var user = JsonConvert.DeserializeObject<AppUserModel>(await userResponse.Content.ReadAsStringAsync());

            var messModel = new MessageModel
            {
                ChatModelId = chatId,
                Message = message,
                UserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                UserName = user.UserName,
                CreatedDate = DateTime.Now,
            };

            var messJson = JsonConvert.SerializeObject(messModel);
            var buffer = Encoding.UTF8.GetBytes(messJson);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("/api/Message", byteContent);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Fail"] = "Wystąpił problem i wiadomość nie została wysłana";
                return RedirectToAction("Index", new { id = teamId });
            }

            await hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", new
            {
                ChatModelId = messModel.ChatModelId,
                Message = messModel.Message,
                UserId = messModel.UserId,
                UserName = messModel.UserName + ": ",
                CreatedDate = messModel.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss")
            });
            return Ok();
        }
    }
}
