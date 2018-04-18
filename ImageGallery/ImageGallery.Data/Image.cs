using System;
using System.Collections.Generic;
using System.Text;

namespace ImageGallery.Data
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string  FileName { get; set; }
        public string OwnerId { get; set; }
    }
}
