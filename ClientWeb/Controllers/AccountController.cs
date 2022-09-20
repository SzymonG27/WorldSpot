using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace ClientWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;
        public AccountController(IConfiguration configuration)
        {
            this.configuration = configuration;
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
    }
}
