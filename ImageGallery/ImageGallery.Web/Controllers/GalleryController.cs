using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace ImageGallery.Web.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {

        public GalleryController(ImageService imageService, HttpService httpService)
        {
            ImageService = imageService;
            HttpService = httpService;
        }

        public ImageService ImageService { get; }
        public HttpService HttpService { get; }
        public IConfiguration Configuration { get; }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

            var images = await ImageService.GetImagesAsync();
            var indexImageViewModel = new IndexImageViewModel()
            {
                Images = images
            };

            return View(indexImageViewModel);
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
            var image = await ImageService.GetImageAsync(id);

            var editImageViewModel = new EditImageViewModel()
            {
                Id = image.Id,
                Title = image.Title
            };

            return View(editImageViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var image = await ImageService.GetImageAsync(editImageViewModel.Id);

            await ImageService.UpdateImageAsync(image.Id, image);

            return RedirectToAction(nameof(Index));
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