using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ImageGallery.DTO
{
    public class ImageCreatorDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; }
        [Required]
        public byte[] Bytes { get; set; }
    }
}
