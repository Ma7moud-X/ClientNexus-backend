using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models.Content
{
    public class DocumentCategory
    {
        public int DocumentId { get; set; }
        public Document Document { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}