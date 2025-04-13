using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class DocumentResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string? ImageUrl { get; set; }
        public string Content { get; set; } 
        public List<string> Categories { get; set; } = new List<string>();
    }
}
