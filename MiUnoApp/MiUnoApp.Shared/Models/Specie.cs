using System;
using System.Collections.Generic;
using System.Text;

namespace MiUnoApp.Models
{
    public class Specie
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Pet> Pets { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
