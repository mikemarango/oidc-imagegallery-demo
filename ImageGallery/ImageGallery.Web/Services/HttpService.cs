using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Web.Services
{
    public class HttpService : IHttpService
    {
        public HttpService(IHttpContextAccessor httpContext, HttpClient httpClient, IConfiguration configuration)
        {
            HttpContext = httpContext;
            HttpClient = httpClient;
            Configuration = configuration;
        }

        public IHttpContextAccessor HttpContext { get; }
        public HttpClient HttpClient { get; }
        public IConfiguration Configuration { get; }

        public async Task<HttpClient> GetClient()
        {

            var accessToken = await GetValidAccessToken();
            if (!string.IsNullOrEmpty(accessToken))
            {
                HttpClient.SetBearerToken(accessToken);
            }

            //HttpClient.BaseAddress = new Uri(Configuration.GetConnectionString("imageApiUri"));
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return HttpClient;
        }

        private async Task<string> GetValidAccessToken()
        {
            var currentContext = HttpContext.HttpContext;
            var expiresAtToken = await currentContext.GetTokenAsync("expires_at");
            var expiresAt = string.IsNullOrWhiteSpace(expiresAtToken) ? 
                DateTime.MinValue : DateTime.Parse(expiresAtToken).AddSeconds(-60).ToUniversalTime();
            var accessToken = await (expiresAt < DateTime.UtcNow ? 
                RenewTokens() : currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken));
            return accessToken;
        }

        private async Task<string> RenewTokens()
        {
            var currentContext = HttpContext.HttpContext;
            var discoveryClient = new DiscoveryClient(Configuration.GetConnectionString("identityServerUri"));
            var metaDataResponse = await discoveryClient.GetAsync();
            var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, Configuration["ClientIdOnIdentityServer"], Configuration["ClientSecretOnIdentityServer"]);
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);
            if (!tokenResult.IsError)
            {
                // get current tokens
                var old_id_token = await currentContext.GetTokenAsync("id_token");
                var new_access_token = tokenResult.AccessToken;
                var new_refresh_token = tokenResult.RefreshToken;

                // get new tokens and expiration time
                var tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = old_id_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = new_access_token },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = new_refresh_token }
                };

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) });

                // store tokens and sign in with renewed tokens
                var info = await currentContext.AuthenticateAsync("Cookies");
                info.Properties.StoreTokens(tokens);
                await currentContext.SignInAsync("Cookies", info.Principal, info.Properties);

                // return the new access token 
                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception("Problem encountered while refreshing tokens.",
                    tokenResult.Exception);
            }
        }
    }
}
