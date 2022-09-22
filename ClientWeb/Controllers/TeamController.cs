﻿using ClientWeb.Models;
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

                HttpResponseMessage response = await client.PostAsync("api/Team", byteContent); //Dodawanie teamu do bazy
                if (response.IsSuccessStatusCode)
                {
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
            var token = await tokenService.GetToken("WorldSpotAPI.read");

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
                        teamsModel.Add(teams.FirstOrDefault(r => r.Id == teamRel.Id)); //Dodawanie do wyświetlanych teamów, w których użytkownik jest
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
            var token = await tokenService.GetToken("WorldSpotAPI.read");

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
    }
}
