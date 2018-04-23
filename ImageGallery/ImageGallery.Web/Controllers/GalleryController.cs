using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Data;
using ImageGallery.DTO;
using ImageGallery.Web.Models;
using ImageGallery.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System.Text;

namespace ImageGallery.Web.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {

        public GalleryController(HttpService httpService)
        {
            HttpService = httpService;
        }

        public ImageService ImageService { get; }
        public HttpService HttpService { get; }
        public IConfiguration Configuration { get; }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

            var httpClient = await HttpService.GetClient();
            var response = await httpClient.GetAsync("api/image");
            //response.EnsureSuccessStatusCode();
            return await HandleApiResponseAsync(response, async () =>
            {
                var images = await response.Content.ReadAsStringAsync();
                var indexImageViewModel = new IndexImageViewModel()
                {
                    Images = JsonConvert.DeserializeObject<IList<Image>>(images).ToList()
                };
                return View(indexImageViewModel);
            });
        }

        private async Task<IActionResult> HandleApiResponseAsync(HttpResponseMessage response, Func<Task<IActionResult>> onSuccess)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    {
                        return await onSuccess();
                    }
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    return RedirectToAction("AccessDenied", "Account");
                default:
                    throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
            }
        }

        private IActionResult HandleApiResponse(HttpResponseMessage response, Func<IActionResult> onSuccess)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.Created:
                    {
                        return onSuccess();
                    }
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    return RedirectToAction("AccessDenied", "Account");
                default:
                    throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
            }

        }

        public async Task<IActionResult> Create()
        {
            return await Task.FromResult(View());
        }

        public async Task<IActionResult> Create(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var image = new Image()
            {
                Title = editImageViewModel.Title
            };

            await ImageService.AddImageAsync(image);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var httpClient = await HttpService.GetClient();
            var response = await httpClient.GetAsync($"api/image/{id}");
            return await HandleApiResponseAsync(response, async () =>
            {
                var imageAsString = await response.Content.ReadAsStringAsync();
                var deserializeImage = JsonConvert.DeserializeObject<Image>(imageAsString);
                var editImageViewModel = new EditImageViewModel()
                {
                    Id = deserializeImage.Id,
                    Title = deserializeImage.Title
                };

                return View(editImageViewModel);
            });

            //var editImageViewModel = new EditImageViewModel()
            //{
            //    Id = image.Id,
            //    Title = image.Title
            //};

            //return View(editImageViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var imageUpdaterDto = new ImageUpdaterDto()
            {
                Title = editImageViewModel.Title
            };

            var serializedImageUpdateDto = JsonConvert.SerializeObject(imageUpdaterDto);

            var httpService = await HttpService.GetClient();

            var response = await httpService.PutAsJsonAsync($"api/image/{editImageViewModel.Id}", imageUpdaterDto)
                .ConfigureAwait(false);

            return HandleApiResponse(response, () => RedirectToAction("Index"));
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            Debug.WriteLine($"IdentityToken: {identityToken}");
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type}, claim value: {claim.Value}");
            }
        }

        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> Order()
        {
            //await HttpService.GetClient();
            var discoveryClient = new DiscoveryClient("https://localhost:44361/");
            var metaDataResponse = await discoveryClient.GetAsync();
            var userInfoClient = new UserInfoClient(metaDataResponse.UserInfoEndpoint);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await userInfoClient.GetAsync(accessToken);
            if (response.IsError)
            {
                throw new Exception("Problem accessing UserInfo endpoint");
            }

            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            return View(new OrderViewModel(address));
        }

    }
}