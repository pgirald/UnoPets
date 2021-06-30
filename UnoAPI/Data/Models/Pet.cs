using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UnoAPI.Data.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public User Owner { get; set; }

        public Specie Kind { get; set; }
    }
}
