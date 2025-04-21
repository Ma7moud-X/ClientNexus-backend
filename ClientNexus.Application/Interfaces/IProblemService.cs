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
        Task<IEnumerable<ProblemListItemDto>> GetClientProblemsAsync(int clientId);
        Task<IEnumerable<ProblemListItemDto>> GetServiceProviderProblemsAsync(int serviceProviderId);
        Task<ProblemListItemDto> GetProblemByIdAsync(int id);
        Task<ProblemListItemDto> UpdateProblemAsync(UpdateProblemDto updateProblemDto, int id);
        Task<bool> DeleteProblemAsync(int id);
        Task<IEnumerable<ProblemAdminDto>> GetAllProblemsAsync();
        Task<ProblemAdminDto> GetProblemAdminDetailsAsync(int id);
        Task<ProblemAdminDto> UpdateProblemStatusAsync(int id, UpdateProblemStatusDto statusDto, int adminId);
        Task<ProblemAdminDto> AddAdminCommentAsync(int id, AdminCommentDto commentDto);
    }
}
