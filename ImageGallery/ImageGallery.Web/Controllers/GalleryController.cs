using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public GalleryController(ImageService service)
        {
            Service = service;
        }

        public ImageService Service { get; }
        public IConfiguration Configuration { get; }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

            var images = await Service.GetImagesAsync();
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

            await Service.AddImageAsync(image);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var image = await Service.GetImageAsync(id);

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

            var image = await Service.GetImageAsync(editImageViewModel.Id);

            await Service.UpdateImageAsync(image.Id, image);

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

    }
}