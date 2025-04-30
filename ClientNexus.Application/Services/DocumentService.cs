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
        private readonly IFileService fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _bucketName;
        private readonly ICategoryService _categoryService;
        
        public DocumentService(IFileService fileService, IUnitOfWork unitOfWork,ICategoryService categoryService)
        {
              this.fileService= fileService;
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
                DocumentCategories = new List<DocumentCategory>() 

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

                string key = $"{Guid.NewGuid()}.{extension}";

                document.ImageUrl = await fileService.UploadPublicFileAsync(stream, fileType,key);
            }
            _unitOfWork.Documents.AddAsync(document);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message;
                throw new Exception($"SaveChanges failed: {ex.Message}, Inner: {inner}", ex);
            }
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message;
                throw new Exception($"SaveChanges failed: {ex.Message}, Inner: {inner}", ex);
            }

            //await _unitOfWork.SaveChangesAsync();

            await _categoryService.AddCategoriesToDocument(document.DocumentCategories, dto.CategoryIds, document.Id);
            await _unitOfWork.SaveChangesAsync();
            var categoryNames = (await _unitOfWork.DCategories.GetAllQueryable()
                .Where(c => dto.CategoryIds.Contains(c.Id))
                  .Select(c => c.Name)
                    .ToListAsync());
            return new DocumentResponseDTO
            {
                Id = document.Id,
                Title = document.Title,
                ImageUrl = document.ImageUrl,
                Content = document.Content,
                Categories = categoryNames
            };
        }
        public async Task DeleteDocumentAsync(int documentId)
        {


            var document = await _unitOfWork.Documents.FirstOrDefaultAsync(d=>d.Id==documentId) ;




            if (document == null)
            {
                throw new KeyNotFoundException("Document not found.");

            }

            if (!string.IsNullOrEmpty(document.ImageUrl))
            {
                var fileKey = document.ImageUrl.Split('/').Last();

                await fileService.DeleteFileAsync(fileKey);

            }


            _unitOfWork.Documents.Delete(document);
            await _unitOfWork.SaveChangesAsync();

        }

 public async Task<DocumentResponseDTO> GetDocumentByIdAsync(int documentId)
{
    // Fetch the document along with its associated categories and category details
    var document = await _unitOfWork.Documents
        .GetAllQueryable()
        .Where(d => d.Id == documentId)
        .Include(d => d.DocumentCategories)
            .ThenInclude(dc => dc.Category) 
        .FirstOrDefaultAsync();

    if (document == null)
    {
        throw new KeyNotFoundException("Document not found.");
    }

    // Extract the category names directly
    var categoryNames = document.DocumentCategories
        .Select(dc => dc.Category.Name)
        .ToList();

    // Return the document response
    return new DocumentResponseDTO
    {
        Id = document.Id,
        Title = document.Title,
        Content = document.Content,
        ImageUrl = document.ImageUrl,
        Categories = categoryNames
    };
}

        public async Task<List<DocumentResponseDTO>> GetAllDocumentsAsync()
        {
            // Fetch all documents with their related document categories
            var documents = await _unitOfWork.Documents
                .GetAllQueryable()
                .Include(d => d.DocumentCategories)
                .ToListAsync();

            // Fetch all categories once
            var allCategories = await _unitOfWork.DCategories
                .GetAllQueryable()
                .ToListAsync();

            // Map documents to response DTOs
            var documentResponses = documents.Select(document =>
            {
                var categoryNames = document.DocumentCategories
                    .Select(dc => allCategories.FirstOrDefault(c => c.Id == dc.DCategoryId)?.Name ?? "")
                    .ToList();

                return new DocumentResponseDTO
                {
                    Id = document.Id,
                    Title = document.Title,
                    Content = document.Content,
                    ImageUrl = document.ImageUrl,
                    Categories = categoryNames
                };
            }).ToList();

            return documentResponses;
        }








    }
}
