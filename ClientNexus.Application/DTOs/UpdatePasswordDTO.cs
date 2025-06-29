﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class UpdatePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
