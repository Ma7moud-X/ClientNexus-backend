using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IDocumentService
    {
        public Task<DocumentResponseDTO> AddDocumentAsync(DocumentDTO dto);
        public Task DeleteDocumentAsync(int documentId);
        public Task<DocumentResponseDTO> GetDocumentByIdAsync(int documentId);
        public Task<List<DocumentResponseDTO>> GetAllDocumentsAsync();


    }
}
