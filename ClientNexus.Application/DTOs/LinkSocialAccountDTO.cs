using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class LinkSocialAccountDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string AccessToken { get; set; } 

        [Required]
        public string Provider { get; set; } // "Google" or "Facebook"
    }
}
