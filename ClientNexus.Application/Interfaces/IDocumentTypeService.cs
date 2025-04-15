using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IDocumentTypeService
    {
        public  Task AddDocumentTypeAsync(string DocumentTypeName);
        public Task DeleteDocumentTypeAsync(int DocumentTypeId);
    }
}
