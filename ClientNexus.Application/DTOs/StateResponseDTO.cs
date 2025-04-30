using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class StateResponseDTO
    {

        public int Id { get; set; }
       
        public string Name { get; set; }
        public string? Abbreviation { get; set; }
        public int CountryId { get; set; }

    }
}
