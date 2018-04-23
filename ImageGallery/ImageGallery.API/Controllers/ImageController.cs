using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImageGallery.Api.Services.Repositories;
using ImageGallery.Data;
using ImageGallery.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.API.Controllers
{
    [Route("api/image")]
    [Authorize]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public ImageController(IGalleryRepository repository, IHostingEnvironment environment)
        {
            Repository = repository;
            Environment = environment;
        }

        public IGalleryRepository Repository { get; }
        public IHostingEnvironment Environment { get; }

        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await Repository.GetImagesAsync();

            var imageDto = Mapper.Map<IEnumerable<ImageDto>>(images);

            return Ok(imageDto);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            if (id == null) return BadRequest();

            var image = await Repository.GetImageAsync(id);

            if (image == null) return NotFound();

            var imageDto = Mapper.Map<ImageDto>(image);

            return Ok(imageDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostImage([FromBody]ImageCreatorDto imageCreatorDto)
        {
            if (imageCreatorDto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var image = Mapper.Map<Image>(imageCreatorDto);

            var webRootPath = Environment.WebRootPath;

            var fileName = $"{Guid.NewGuid().ToString()}.jpg";

            var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

            await System.IO.File.WriteAllBytesAsync(fileName, imageCreatorDto.Bytes);

            image.FileName = fileName;

            await Repository.AddImageAsync(image);

            var imageDto = Mapper.Map<ImageDto>(image);

            return CreatedAtRoute("GetImage", new { id = imageDto }, imageDto);
        }

        [HttpPut]
        public async Task<IActionResult> PutImage(Guid id, [FromBody]ImageUpdaterDto imageUpdaterDto)
        {
            if (imageUpdaterDto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var image = await Repository.GetImageAsync(id);

            if (image == null)
                return NotFound();

            Mapper.Map(imageUpdaterDto, image);

            await Repository.UpdateImageAsync(image);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            if (id == null)
                return BadRequest();

            var image = await Repository.GetImageAsync(id);

            await Repository.DeleteImage(image);

            return NoContent();
        }
    }
}
