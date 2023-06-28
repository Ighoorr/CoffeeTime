using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCoffee.Models
{
    public class EmailDTO
    {
        public string To { get; set; } = string.Empty;
        
        public string Body { get; set; } = string.Empty;
    }
}
