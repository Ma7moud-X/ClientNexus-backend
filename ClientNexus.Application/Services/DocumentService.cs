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

                
                //FileType fileType;

                //switch (extension)
                //{
                //    case "jpg":
                //    case "jpeg":
                //        fileType = FileType.Jpeg;
                //        break;
                //    case "png":
                //        fileType = FileType.Png;
                //        break;
                //    default:
                //        throw new ArgumentException("Unsupported image type");
                //}

                string key = $"{Guid.NewGuid()}.{extension}";

                document.ImageUrl = await fileService.UploadPublicFileAsync(stream, FileType.Png,key);
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



    }
}
