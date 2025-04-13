using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using ClientNexus.Domain.Enums;
namespace ClientNexus.Application.Services
{

    public class DocumentService:IDocumentService
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _bucketName;
        private readonly ICategoryService _categoryService;
        
        public DocumentService(IFileStorage fileStorage, IUnitOfWork unitOfWork,ICategoryService categoryService)
        {
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            _categoryService = categoryService;
        }
        public async Task<DocumentResponseDTO> AddDocumentAsync(DocumentDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "The document DTO cannot be null.");
            }
            if (dto.CategoryIds == null || !dto.CategoryIds.Any())
            {
                throw new ArgumentNullException(nameof(dto.CategoryIds), "Category IDs are required for Document.");
            }

            DocumentType documenttype = await _unitOfWork.DocumentTypes.FirstOrDefaultAsync(d => d.Name.ToLower() == "article");

            var document = new Document
            {
                Title = dto.Title,
                Content = dto.Content,
                DocumentTypeId = documenttype.Id,
                UploadedById = dto.UploadedById,    
            };


            if (dto.ImageFile != null)
            {
                using var stream = dto.ImageFile.OpenReadStream();

             
                string extension = Path.GetExtension(dto.ImageFile.FileName).TrimStart('.').ToLower();

                
                FileType fileType;

                switch (extension)
                {
                    case "jpg":
                    case "jpeg":
                        fileType = FileType.Jpeg;
                        break;
                    case "png":
                        fileType = FileType.Png;
                        break;
                    default:
                        throw new ArgumentException("Unsupported image type");
                }

                string key = $"Images/{Guid.NewGuid()}.{extension}";

                document.ImageUrl = await _fileStorage.UploadFileAsync(stream, key, fileType);
            }
            _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _categoryService.AddCategoriesToDocument(document.DocumentCategories, dto.CategoryIds, document.Id);
            await _unitOfWork.SaveChangesAsync();
            return new DocumentResponseDTO
            {
                Id = document.Id,
                Title = document.Title,
                ImageUrl = document.ImageUrl,
                Content = document.Content,
                Categories = document.DocumentCategories.Select(dc => dc.Category?.Name).ToList()
            };
        }
        public async Task DeleteDocumentAsync(int documentId)
        {
            var document = await _unitOfWork.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
            {
                throw new KeyNotFoundException("Document not found.");

            }

            if (!string.IsNullOrEmpty(document.ImageUrl))
            {
                var fileKey = document.ImageUrl.Split('/').Last(); 
               
                    await _fileStorage.DeleteFileAsync(fileKey); 
                                 
            }

           
            _unitOfWork.Documents.Delete(document);
            await _unitOfWork.SaveChangesAsync();

        }
       


    }
}
