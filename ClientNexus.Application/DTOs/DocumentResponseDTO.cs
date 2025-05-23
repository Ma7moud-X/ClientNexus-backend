﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class DocumentResponseDTO
    {
        [Required]

        public int Id { get; set; }
        [Required]

        public string Title { get; set; }
       
        public string? ImageUrl { get; set; }
        [Required]

        public string Content { get; set; }
        [Required]

        public List<string> Categories { get; set; } = new List<string>();
    }
}
