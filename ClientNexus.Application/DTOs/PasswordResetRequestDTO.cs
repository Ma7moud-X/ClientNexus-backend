using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class PasswordResetRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Otp { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
