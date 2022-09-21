using ClientWeb.Models;
using ClientWeb.Services;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ClientWeb.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        public TeamController(ITokenService tokenService, IConfiguration configuration)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamModel model, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var token = await tokenService.GetToken("WorldSpotAPI.read");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                //Najpierw sprawdzamy czy istnieje nazwa takiego teamu w bazie oraz czy użytkownik nie ma już założonego
                var validate = await client.GetAsync("/api/Team/Check/" + userId + "&" + model.Name);
                if (validate.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var stringContent = await validate.Content.ReadAsStringAsync();
                    var details = JsonConvert.DeserializeObject<ProblemModel>(stringContent);
                    TempData["Fail"] = details.detail;
                    return View(model);
                }

                model.FounderId = userId;
                var teamValues = JsonConvert.SerializeObject(model);
                var buffer = Encoding.UTF8.GetBytes(teamValues);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");



            }

            return View();
        }
    }
}
