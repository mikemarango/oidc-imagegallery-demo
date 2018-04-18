using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ImageGallery.DTO
{
    public class ImageUpdaterDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; }
    }
}
