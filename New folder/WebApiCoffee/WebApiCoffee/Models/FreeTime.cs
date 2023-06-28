using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiCoffee.Models
{
    public partial class FreeTime
    {
        public int TimeId { get; set; }
        public int? UserId { get; set; }
        public DateTime? Time { get; set; }
        public string Day { get; set; }

        public virtual User User { get; set; }
    }
}
