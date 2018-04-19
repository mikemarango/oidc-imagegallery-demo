using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Web.Models
{
    public class CreateImageViewModel
    {
        public string Title { get; set; }
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
