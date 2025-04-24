using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
   public class DocumentTypeService:IDocumentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentTypeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task AddDocumentTypeAsync(string DocumentTypeName)
        {
            if (string.IsNullOrWhiteSpace(DocumentTypeName))
            {
                throw new ArgumentException("Document Type name cannot be empty.");
            }

            var existingDocumentType = await _unitOfWork.DocumentTypes
                     .FirstOrDefaultAsync(c => c.Name.ToLower() == DocumentTypeName.ToLower());


            if (existingDocumentType != null)
            {
                throw new InvalidOperationException("Document Type with the same name already exists.");
            }

            var DocumentType = new DocumentType
            {
                Name = DocumentTypeName
            };

            await _unitOfWork.DocumentTypes.AddAsync(DocumentType);
            await _unitOfWork.SaveChangesAsync();
           
        }
        public async Task DeleteDocumentTypeAsync(int DocumentTypeId)
        {
            var DocumentType = await _unitOfWork.DocumentTypes
                .FirstOrDefaultAsync(c => c.Id == DocumentTypeId);

            if (DocumentType == null)
            {
                throw new KeyNotFoundException($"DocumentType with ID {DocumentTypeId} not found.");
            }



            _unitOfWork.DocumentTypes.Delete(DocumentType);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
