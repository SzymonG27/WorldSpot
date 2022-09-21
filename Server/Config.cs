using IdentityServer4.Models;

namespace Server
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { "role" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[] { new ApiScope("WorldSpotAPI.read"), new ApiScope("WorldSpotAPI.write"), };
        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("WorldSpotAPI")
                {
                    Scopes = new List<string> { "WorldSpotAPI.read", "WorldSpotAPI.write" },
                    ApiSecrets = new List<Secret> { new Secret("Cnmpw23890dfhwfehr30rfb02r".Sha256()) },
                    UserClaims = new List<string> { "role" }
                }
            };
        public static IEnumerable<Client> Clients =>
           new[]
           {
                new Client
                {
                    ClientId = "m2m.client",    //serwer - serwer
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "WorldSpotAPI.read", "WorldSpotAPI.write" }
                },
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5444/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5444/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "WorldSpotAPI.read" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                },
           };
    }
}
