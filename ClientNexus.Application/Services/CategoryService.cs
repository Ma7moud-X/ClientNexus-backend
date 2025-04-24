using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task AddCategoriesToDocument(ICollection<DocumentCategory> DocumentCategories, List<int> CategoryIDs, int DocumentId)
        {
           
            //  Validate category IDs exist in the database
            var validCategoryIds = (_unitOfWork.DCategories.GetAllQueryable()).Select(C =>C.Id ).ToList();
            var invalidCategoryIds = CategoryIDs.Where(id => !validCategoryIds.Contains(id)).ToList();
            if (invalidCategoryIds.Any())
            {
                throw new ArgumentException($"Invalid Category IDs: {string.Join(", ", invalidCategoryIds)}");
            }
            foreach (var catId in CategoryIDs)
            {
                if (!DocumentCategories.Any(d => d.DCategoryId == catId))
                {
                   DocumentCategories.Add(new DocumentCategory
                    {
                        DCategoryId = catId,
                        DocumentId = DocumentId // Ensure the relationship is set correctly
                    });
                }
            }
            await _unitOfWork.SaveChangesAsync();


        }
        public async Task<CategoryResponseDTO> AddCategoryAsync(string CategoryName)
        {
            if (string.IsNullOrWhiteSpace(CategoryName))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            var existingCategory = await _unitOfWork.DCategories
                     .FirstOrDefaultAsync(c => c.Name.ToLower() == CategoryName.ToLower());


            if (existingCategory != null)
            {
                throw new InvalidOperationException("Category with the same name already exists.");
            }

            var newCategory = new DCategory
            {
                Name = CategoryName
            };

           await _unitOfWork.DCategories.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            return new CategoryResponseDTO
            {
                Id = newCategory.Id,
                Name = CategoryName

            };
            
        }
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.DCategories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

   

            _unitOfWork.DCategories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
