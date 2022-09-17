using IdentityModel.Client;

namespace ClientWeb.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}
