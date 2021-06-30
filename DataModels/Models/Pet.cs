using System;
using System.Collections.Generic;
using System.Text;

namespace DataModels.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Breed { get; set; }

        public string Specie { get; set; }

        public User Owner { get; set; }

        public Specie Kind { get; set; }
    }
}
