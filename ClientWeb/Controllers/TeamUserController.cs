using ClientWeb.Models;
using ClientWeb.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ClientWeb.Controllers
{
    public class TeamUserController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        public TeamUserController(ITokenService tokenService, IConfiguration configuration)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> List(int teamId, string returnUrl)
        {
            var token = await tokenService.GetToken("WorldSpotAPI.read");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var response = await client.GetAsync("/api/TeamUsersRelation");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie póżniej";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var teamsRelation = JsonConvert.DeserializeObject<IEnumerable<TeamUsersRelationModel>>(responseString);


                var usersInTeamRel = new List<TeamUsersRelationModel>();
                foreach (var relation in teamsRelation)
                {
                    if (relation.TeamId == teamId)
                    {
                        usersInTeamRel.Add(relation);
                    }
                }
                if (usersInTeamRel.Count <= 0)
                {
                    TempData["Fail"] = "Nie masz żadnego członka w teamie";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                return View(usersInTeamRel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returnUrl)
        {
            var token = await tokenService.GetToken("WorldSpotAPI.read");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration["apiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tokenResponse = await tokenService.GetToken("WorldSpotAPI.read");
                client.SetBearerToken(tokenResponse.AccessToken);

                var response = await client.DeleteAsync("/api/TeamUsersRelation/" + id);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Fail"] = "Wystąpił błąd! Spróbuj ponownie później";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                TempData["Success"] = "Użytkownik został pomyślnie usunięty z teamu!";
                return RedirectToAction("List", "Team"); //Nie używam returnUrl ponieważ nie wiadomo czy są jeszcze jakieś relacje po usunięciu, tempdata może się zduplikować. Nie chce obciążać funkcji delete niepotrzebnym sprawdzaniem tego.

            }
        }
    }
}
