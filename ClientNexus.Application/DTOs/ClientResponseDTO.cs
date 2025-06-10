using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class ClientResponseDTO
    {
        
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string PhoneNumber { get; set; } 
        public DateOnly BirthDate { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string MainImage { get; set; }

    }
}
