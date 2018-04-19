using ImageGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Web.Models
{
    public class IndexImageViewModel
    {
        public IEnumerable<Image> Images { get; set; } = new List<Image>();
    }
}
