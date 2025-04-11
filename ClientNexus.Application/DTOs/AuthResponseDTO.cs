using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string UserType { get; set; } = default!;
    }

}
