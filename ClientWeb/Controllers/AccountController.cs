using ClientWeb.Models;
using ClientWeb.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ClientWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        public AccountController(IConfiguration configuration, ITokenService tokenService)
        {
            this.configuration = configuration;
            this.tokenService = tokenService;
        }

        public IActionResult Login(string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                redirectUri = Url.Content("~/");
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect(redirectUri);
            }

            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectUri,
            }, OpenIdConnectDefaults.AuthenticationScheme);
            
        }

        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = configuration["applicationUrl"]
            }, 
            OpenIdConnectDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id, string returnUrl)
        {
            var token = await tokenService.GetToken("WorldSpotAPI.read");

            if (string.IsNullOrWhiteSpace(id))
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

                var response = await client.GetAsync("/api/Account/" + id);
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
                    TempData["Fail"] = "Nie znaleziono użytkownika o podanym id";
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<AppUserModel>(responseString);
                return View(user);
            }
        }
    }
}
