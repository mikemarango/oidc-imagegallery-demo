using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Web.Models
{
    public class OrderViewModel
    {
        public string Address { get; set; } = string.Empty;

        public OrderViewModel(string address)
        {
            Address = address;
        }
    }
}
