using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Content;
using ClientNexus.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface ICategoryService
    {
        public Task AddCategoriesToDocument(ICollection<DocumentCategory> DocumentCategories, List<int> CategoryIDs, int DocumentId);
        public  Task<CategoryResponseDTO> AddCategoryAsync(string CategoryName);
        public Task DeleteCategoryAsync(int categoryId);
        public Task<List<CategoryResponseDTO>> GetAllStatesAsync();



    }
}
