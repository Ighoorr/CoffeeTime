using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiCoffee.Models
{
    public partial class Photo
    {
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public int? UserId { get; set; }

        
    }
}
