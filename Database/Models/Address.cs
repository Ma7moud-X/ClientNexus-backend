using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models
{
     public class Address
    {
        public int Id { get; set; }
        public string DetailedAddress { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
    }
}