using ImageGallery.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Api.Services.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        public GalleryContext Context { get; }

        public GalleryRepository(GalleryContext context)
        {
            Context = context;
        }

        public Task<List<Image>> GetImagesAsync()
        {
            return Context.Images.ToListAsync();
        }

        public Task<Image> GetImageAsync(Guid id)
        {
            return Context.Images.FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task AddImageAsync(Image image)
        {
            Context.Images.Add(image);
            return Context.SaveChangesAsync();
        }

        public Task UpdateImageAsync(Image image)
        {
            Context.Entry(image).State = EntityState.Modified;
            return Context.SaveChangesAsync();
        }

        public Task DeleteImage(Image image)
        {
            Context.Images.Remove(image);
            return Context.SaveChangesAsync();
        }
    }
}
