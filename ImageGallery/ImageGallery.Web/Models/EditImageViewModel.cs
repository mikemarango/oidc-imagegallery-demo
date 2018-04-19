using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Web.Models
{
    public class EditImageViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required, Display(Name = "Image Title")]
        public string Title { get; set; }
    }
}
