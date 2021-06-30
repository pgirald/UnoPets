using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace UnoAPI.Data.Models
{
    public class User : IdentityUser
    {
        [MaxLength(20)]
        public string FirstNames { get; set; }

        [MaxLength(20)]
        public string SecondNames { get; set; }

        public ICollection<Pet> Pets { get; set; }
    }
}
