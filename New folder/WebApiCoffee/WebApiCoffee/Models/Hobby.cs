using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiCoffee.Models
{
    public partial class Hobby
    {
        public int HobbyId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public virtual User User { get; set; }
    }
}
