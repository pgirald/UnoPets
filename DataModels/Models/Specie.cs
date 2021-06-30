using System;
using System.Collections.Generic;
using System.Text;

namespace DataModels.Models
{
    public class Specie
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Pet> Pets { get; set; }
    }
}
