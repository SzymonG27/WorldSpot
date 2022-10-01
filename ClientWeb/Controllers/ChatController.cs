using ClientWeb.Models;
using ClientWeb.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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

        [HttpGet]
        public async Task<IActionResult> Index(int id, string returnUrl)
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
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Fail"] = "Najpierw musisz być zalogowany!";
                return RedirectToAction("Login", "Account");
            }
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userName = User.Identity.Name;
            var messModel = new MessageModel
            {
                ChatModelId = chatId,
                Message = message,
                UserId = userId,
                UserName = userName,
                CreatedDate = DateTime.Now,
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

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
            return RedirectToAction("Index", new { id = chatId });
        }
    }
}
