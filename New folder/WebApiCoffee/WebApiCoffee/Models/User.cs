using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiCoffee.Models
{
    public partial class User
    {
        public User()
        {
            FreeTimes = new HashSet<FreeTime>();
            Hobbies = new HashSet<Hobby>();
           
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Pass { get; set; }

        public virtual ICollection<FreeTime> FreeTimes { get; set; }
        public virtual ICollection<Hobby> Hobbies { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
