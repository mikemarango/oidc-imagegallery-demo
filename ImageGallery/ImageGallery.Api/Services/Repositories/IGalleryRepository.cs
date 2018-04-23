using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageGallery.Data;

namespace ImageGallery.Api.Services.Repositories
{
    public interface IGalleryRepository
    {
        Task AddImageAsync(Image image);
        Task DeleteImage(Image image);
        Task<Image> GetImageAsync(Guid id);
        Task<List<Image>> GetImagesAsync(string ownerId);
        Task UpdateImageAsync(Image image);
    }
}