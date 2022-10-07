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
        public IActionResult Create(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamModel model, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
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

                HttpResponseMessage response = await client.PostAsync("api/Team", byteContent); //Dodawanie teamu do bazy
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = JsonConvert.DeserializeObject<TeamModel>(await response.Content.ReadAsStringAsync());

                    //Tworzymy relacje teamu z użytkownikiem
                    var relationJson = JsonConvert.SerializeObject(new TeamUsersRelationModel { 
                        TeamId = responseContent.Id,
                        UserId = userId
                    });
                    var bufferRelation = Encoding.UTF8.GetBytes(relationJson);
                    var byteContentRelation = new ByteArrayContent(bufferRelation);
                    byteContentRelation.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var createTeam = await client.PostAsync("/api/TeamUsersRelation", byteContentRelation);
                    if (!createTeam.IsSuccessStatusCode)
                    {
                        //TODO: Jak wyskoczy błąd usuwa stworzony team
                        TempData["Fail"] = "Nie dodano użytkownika do relacji! Skontaktuj się z administratorem strony";
                        return RedirectToAction("Index", "Home");
                    }

                    //Tworzymy czat w bazie danych dla teamu
                    var chat = JsonConvert.SerializeObject(new ChatModel { TeamId = responseContent.Id });
                    var chatBuffer = Encoding.UTF8.GetBytes(chat);
                    var chatByteContent = new ByteArrayContent(chatBuffer);
                    chatByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage createChat = await client.PostAsync("/api/Chat", chatByteContent); //Musi być success dlatego bez sprawdzenia

                    TempData["Success"] = "Team został stworzony pomyślnie";

                    object returnUrl = string.Empty;
                    TempData.TryGetValue("ReturnUrl", out returnUrl);
                    string returnUrlStr = returnUrl as string;
                    if (!string.IsNullOrEmpty(returnUrlStr))
                    {
                        return Redirect(returnUrlStr);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> List(string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);


                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await client.GetAsync("/api/Team");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var teams = JsonConvert.DeserializeObject<IEnumerable<TeamModel>>(responseString);
                List<TeamModel> teamsModel = new List<TeamModel>(); //Do widoku dodawane będą tylko rekordy dla użytkownika

                foreach (var team in teams)
                {
                    if (team.FounderId == userId)
                    {
                        teamsModel.Add(team);
                    }
                }

                var responseRel = await client.GetAsync("api/TeamUsersRelation");   //Szukanie relacji z użytkownikiem
                if (!responseRel.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                var responseRelString = await responseRel.Content.ReadAsStringAsync();
                var teamsRel = JsonConvert.DeserializeObject<IEnumerable<TeamUsersRelationModel>>(responseRelString);
                foreach (var teamRel in teamsRel)
                {
                    if (userId == teamRel.UserId)
                    {
                        var add = teams.FirstOrDefault(r => r.Id == teamRel.TeamId);
                        if (add == null)
                        {
                            continue;
                        }
                        if (teamsModel.Contains(add)) //Sprawdza czy team został już dodany do listy (właściciel)
                        {
                            continue;
                        }
                        teamsModel.Add(add); //Dodawanie do wyświetlanych teamów, w których użytkownik jest
                    }
                }

                if (teamsModel.Count <= 0 || teamsModel == null)
                {
                    TempData["Fail"] = "Nie należysz ani nie masz stworzonego żadnego teamu!";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                return View(teamsModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id <= 0)
            {
                TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                //var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await client.GetAsync("/api/Team/" + id);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["Fail"] = "Nie znaleziono teamu o takim id";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var team = JsonConvert.DeserializeObject<TeamModel>(responseString);
                return View(team);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await client.GetAsync("/api/Team/" + id);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var team = JsonConvert.DeserializeObject<TeamModel>(responseString);

                if (userId != team.FounderId)
                {
                    TempData["Fail"] = "Nie jesteś właścicielem teamu! Nie możesz go edytować";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                return View(team);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeamModel model)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != model.FounderId)  //Powielone sprawdzenie
                {
                    TempData["Fail"] = "Nie jesteś właścicielem teamu! Nie możesz go edytować";
                    return RedirectToAction("List");
                }
                var teamValues = JsonConvert.SerializeObject(model);
                var buffer = Encoding.UTF8.GetBytes(teamValues);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PutAsync("/api/Team", byteContent);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                    return RedirectToAction("List");
                }

                TempData["Success"] = "Pomyślnie edytowano dane teamu";
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var checkTeam = await client.GetAsync("/api/Team/" + id); //Sprawdzamy czy użytkownik jest właścicielem teamu
                if (!checkTeam.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie później";
                    return RedirectToAction("List");
                }
                var checkTeamString = await checkTeam.Content.ReadAsStringAsync();
                var team = JsonConvert.DeserializeObject<TeamModel>(checkTeamString);

                if (team.FounderId != userId)
                {
                    TempData["Fail"] = "Nie jesteś właścicielem teamu! Nie możesz go usunąć";
                    return RedirectToAction("List");
                }

                return View(team);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var response = await client.DeleteAsync("/api/Team/" + id);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił błąd! Spróbuj ponownie później";
                    return RedirectToAction("List");
                }
                TempData["Success"] = "Team został pomyślnie usunięty!";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
