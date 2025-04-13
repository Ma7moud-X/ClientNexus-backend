using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class DocumentDTO
    {
        [Required]
        public string Title { get; set; } 
        [Required]
        public string Content { get; set; } 
        //public int DocumentTypeId { get; set; }
        public int? UploadedById { get; set; }
        [Required]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public List<int> CategoryIds { get; set; }
    }
}
