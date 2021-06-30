using System;
using System.Collections.Generic;
using System.Text;

namespace MiUnoApp.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public User Owner { get; set; }

        public Specie Kind { get; set; }
    }
}
