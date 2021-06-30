using System;
using System.Collections.Generic;
using System.Text;

namespace DataModels.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstNames { get; set; }

        public string SecondNames { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<Pet> Pets { get; set; }
    }
}
