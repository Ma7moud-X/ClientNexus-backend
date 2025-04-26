using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.Interfaces
{
    public interface IProblemService
    {
        Task<ProblemListItemDto> CreateProblemAsync(CreateProblemDto createProblemDto);
        Task<IEnumerable<ProblemListItemDto>> GetClientProblemsAsync(int clientId, int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<ProblemListItemDto>> GetServiceProviderProblemsAsync(int serviceProviderId, int pageNumber = 1, int pageSize = 10);
        Task<ProblemListItemDto> GetProblemByIdAsync(int id, int userId, string userRole);
        Task<ProblemListItemDto> UpdateProblemAsync(UpdateProblemDto updateProblemDto, int id, int userId, string userRole);
        Task<bool> DeleteProblemAsync(int id, int userId, string userRole);
        Task<IEnumerable<ProblemAdminDto>> GetAllProblemsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ProblemAdminDto> GetProblemAdminDetailsAsync(int id);
        Task<ProblemAdminDto> UpdateProblemStatusAsync(int id, UpdateProblemStatusDto statusDto, int adminId);
        Task<ProblemAdminDto> AddAdminCommentAsync(int id, AdminCommentDto commentDto);

        int GetTotalProblemCount(int? clientId = null, int? serviceProviderId = null);
    }
}
