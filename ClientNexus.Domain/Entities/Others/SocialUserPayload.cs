﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Domain.Entities.Others
{
    public class SocialUserPayload
    {
        public string Email { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string ProviderId { get; set; } 

    }
}
