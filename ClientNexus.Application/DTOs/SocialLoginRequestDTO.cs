using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class SocialLoginRequestDTO
    {
        
            public string Provider { get; set; }
            public string AccessToken { get; set; }
            public UserType UserType { get; set; }
        
    }
}
