using ClientWeb.Models;
using ClientWeb.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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
        public IActionResult Register(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = new HttpClient();
            client.BaseAddress = new Uri(configuration["apiUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            bool status = false;

            var checkUsername = await client.GetAsync("/api/Account/GetFromNick/" + model.Username);
            if (checkUsername.StatusCode == System.Net.HttpStatusCode.OK) //Dostaje użytkownika
            {
                ModelState.AddModelError(string.Empty, "Użytkownik o takim nicku już istnieje w bazie danych");
                status = true;
            }

            var checkMail = await client.GetAsync("/api/Account/CheckMail/" + model.Mail);
            if (checkMail.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(string.Empty, "Taki mail już istnieje w bazie danych");
                status = true;
            }

            if (status == true)
            {
                return View(model);
            }

            var user = new AppUserModel()
            {
                UserName = model.Username,
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Email = model.Mail,
                DateJoined = DateTime.Now
            };
            var userWithPassword = new UserPasswordModel()
            {
                AppUserModel = user,
                Password = model.Password
            };

            var userValues = JsonConvert.SerializeObject(userWithPassword);
            var buffer = Encoding.UTF8.GetBytes(userValues);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var register = await client.PostAsync("/api/Account", byteContent);
            if (register.IsSuccessStatusCode)
            {
                TempData["Success"] = "Pomyślnie się zarejestrowałeś! Teraz się zaloguj";
                return RedirectToAction("Login");
            }
            TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie lub skontaktuj się z administratorem strony";
            return View(model);
            
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
