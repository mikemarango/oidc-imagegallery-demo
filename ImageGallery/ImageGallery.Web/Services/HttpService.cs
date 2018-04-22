using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Web.Services
{
    public class HttpService
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

            HttpClient.BaseAddress = new Uri(Configuration.GetConnectionString("imageApiUri"));
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
            //var currentContext = HttpContext.HttpContext;
            //var discoveryClient = new DiscoveryClient(Configuration.GetConnectionString("identityServerUri"));
            //var metaDataResponse = await discoveryClient.GetAsync();
            //var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, Configuration["ClientIdOnIdentityServer"], Configuration["ClientSecretOnIdentityServer"]);
            //var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);
            //if (!tokenResult.IsError)
            //{

            //}
            //else
            //{

            //}
            throw new NotImplementedException();
        }
    }
}
