using ImageGallery.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageGallery.Web.Services
{
    public class ImageService
    {
        public ImageService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<IList<Image>> GetImagesAsync()
        {
            var response = await HttpClient.GetAsync("api/image");
            response.EnsureSuccessStatusCode();
            var images = await response.Content.ReadAsAsync<List<Image>>();
            return images;
        }

        public async Task<Image> GetImageAsync(Guid id)
        {
            var response = await HttpClient.GetAsync($"api/image/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            response.EnsureSuccessStatusCode();
            var image = await response.Content.ReadAsAsync<Image>();
            return image;
        }

        public async Task<Image> AddImageAsync(Image image)
        {
            var response = await HttpClient.PostAsJsonAsync($"api/image", image);
            response.EnsureSuccessStatusCode();
            image = await response.Content.ReadAsAsync<Image>();
            return image;
        }

        public async Task UpdateImageAsync(Guid id, Image image)
        {
            var response = await HttpClient.PutAsJsonAsync($"api/image/{id}", image);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Image> DeleteImageAsync(Guid id)
        {
            var response = await HttpClient.DeleteAsync($"api/image/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            var image = await response.Content.ReadAsAsync<Image>();
            return image;
        }
    }
}
