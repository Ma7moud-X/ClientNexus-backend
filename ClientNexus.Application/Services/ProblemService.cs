using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.Application.Services
{
    public class ProblemService : IProblemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProblemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProblemListItemDto> CreateProblemAsync(CreateProblemDto createProblemDto)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(createProblemDto.ClientId) 
            ?? throw new KeyNotFoundException($"Client with ID {createProblemDto.ClientId} not found");

            var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(createProblemDto.ServiceProviderId) 
            ?? throw new KeyNotFoundException($"Service provider with ID {createProblemDto.ServiceProviderId} not found");

            var service = await _unitOfWork.Services.GetByIdAsync(createProblemDto.ServiceId) 
            ?? throw new KeyNotFoundException($"Service with ID {createProblemDto.ServiceId} not found");

            // Create new problem entity
            var problem = new Problem
            {
                Description = createProblemDto.Description,
                Status = ProblemStatus.New,
                ReportedBy = createProblemDto.ReportedBy,
                ClientId = createProblemDto.ClientId,
                ServiceProviderId = createProblemDto.ServiceProviderId,
                ServiceId = createProblemDto.ServiceId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Problems.AddAsync(problem);
            await _unitOfWork.SaveChangesAsync();

            // Map to DTO
            return MapToProblemListItemDto(problem);
        }

        public async Task<IEnumerable<ProblemListItemDto>> GetClientProblemsAsync(int clientId)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId) 
            ?? throw new KeyNotFoundException($"Client with ID {clientId} not found");

            var problems = await _unitOfWork.Problems.GetByConditionAsync(f => f.ClientId == clientId);

            return problems.Select(MapToProblemListItemDto);
        }

        public async Task<IEnumerable<ProblemListItemDto>> GetServiceProviderProblemsAsync(int serviceProviderId)
        {
             var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId) 
            ?? throw new KeyNotFoundException($"Service provider with ID {serviceProviderId} not found");

            var problems = await _unitOfWork.Problems.GetByConditionAsync(f => f.ServiceProviderId == serviceProviderId);

            return problems.Select(MapToProblemListItemDto);
        }

        public async Task<ProblemListItemDto> GetProblemByIdAsync(int id)
        {
            var problem = await _unitOfWork.Problems.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Invalid Problem ID");

            return MapToProblemListItemDto(problem);
        }

        public async Task<ProblemListItemDto> UpdateProblemAsync(UpdateProblemDto updateProblemDto, int id)
        {
            var problem = await _unitOfWork.Problems.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Invalid Problem ID");

            // Only allow updates if the problem hasn't been handled by admin yet
            if (problem.Status != ProblemStatus.New)
            {
                throw new InvalidOperationException("Cannot update problem as it's already being handled by an administrator");
            }

            // Update allowed fields
            problem.Description = updateProblemDto.Description;
            
            await _unitOfWork.SaveChangesAsync();

            return MapToProblemListItemDto(problem);
        }

        public async Task<bool> DeleteProblemAsync(int id)
        {
            var problem = await _unitOfWork.Problems.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Invalid Problem ID");

            // Only allow cancellation if the problem hasn't been handled by admin yet
            if (problem.Status != ProblemStatus.New)
            {
                return false;
            }

            _unitOfWork.Problems.Delete(problem);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProblemAdminDto>> GetAllProblemsAsync()
        {
            var problems = await _unitOfWork.Problems.GetAllQueryable(
                p => p.Client,
                p => p.ServiceProvider,
                p => p.Service).ToListAsync();            
                
            return problems.Select(MapToProblemAdminDto);
        }

        public async Task<ProblemAdminDto> GetProblemAdminDetailsAsync(int id)
        {
            var problems = await _unitOfWork.Problems.GetByConditionAsync(
                p => p.Id == id,
                false,
                0,
                1,
                ["Client", "ServiceProvider", "Service"]
            );
            
            var problem = problems.FirstOrDefault() 
                ?? throw new KeyNotFoundException("Invalid Problem ID");

            return MapToProblemAdminDto(problem);
        }

        public async Task<ProblemAdminDto> UpdateProblemStatusAsync(int id, UpdateProblemStatusDto statusDto, int adminId)
        {
            var problems = await _unitOfWork.Problems.GetByConditionAsync(
                p => p.Id == id,
                false,
                0,
                1,
                ["Client", "ServiceProvider", "Service"]
            );
            
            var problem = problems.FirstOrDefault() 
                ?? throw new KeyNotFoundException("Invalid Problem ID");

            var admin = await _unitOfWork.Admins.GetByIdAsync(adminId) 
            ?? throw new KeyNotFoundException($"Admin with ID {adminId} not found");

            // Update status
            problem.Status = statusDto.Status;
            
            // Assign admin if not already assigned
            problem.SolvingAdminId ??= adminId;
            
            // If the problem is resolved or cancelled, set the closed date
            if (statusDto.Status == ProblemStatus.Done || statusDto.Status == ProblemStatus.Cancelled)
            {
                problem.ClosedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToProblemAdminDto(problem);
        }

        public async Task<ProblemAdminDto> AddAdminCommentAsync(int id, AdminCommentDto commentDto)
        {
            var problems = await _unitOfWork.Problems.GetByConditionAsync(
                p => p.Id == id,
                false,
                0,
                1,
                ["Client", "ServiceProvider", "Service"]
            );
            
            var problem = problems.FirstOrDefault() 
                ?? throw new KeyNotFoundException("Invalid Problem ID");

            // Update comment
            problem.AdminComment = commentDto.AdminComment;
            
            // Assign admin if not already assigned
            problem.SolvingAdminId ??= commentDto.SolvingAdminId;

            await _unitOfWork.SaveChangesAsync();

            return MapToProblemAdminDto(problem);
        }
    
    
        #region Mapping Methods
        
        private ProblemDto MapToProblemDto(Problem problem)
        {
            return new ProblemDto
            {
                Id = problem.Id,
                Description = problem.Description,
                AdminComment = problem.AdminComment,
                Status = problem.Status,
                ReportedBy = problem.ReportedBy,
                ClientId = problem.ClientId,
                ServiceProviderId = problem.ServiceProviderId,
                ServiceId = problem.ServiceId,
                SolvingAdminId = problem.SolvingAdminId,
                CreatedAt = problem.CreatedAt,
                ClosedAt = problem.ClosedAt
            };
        }
        
        private ProblemListItemDto MapToProblemListItemDto(Problem problem)
        { 
            return new ProblemListItemDto
            {
                Id = problem.Id,
                Description = problem.Description,
                AdminComment = problem.AdminComment,
                Status = problem.Status,
                CreatedAt = problem.CreatedAt,
                ClosedAt = problem.ClosedAt
            };
        }
        
        private ProblemAdminDto MapToProblemAdminDto(Problem problem)
        { 
            return new ProblemAdminDto
            {
                Id = problem.Id,
                Description = problem.Description,
                AdminComment = problem.AdminComment,
                Status = problem.Status,
                ReportedBy = problem.ReportedBy,
                ClientId = problem.ClientId,
                ClientName = problem.Client?.FirstName ?? "Unknown",
                ServiceProviderId = problem.ServiceProviderId,
                ServiceProviderName = problem.ServiceProvider?.FirstName ?? "Unknown",
                ServiceId = problem.ServiceId,
                ServiceName = problem.Service?.Name ?? "Unknown",
                CreatedAt = problem.CreatedAt,
                ClosedAt = problem.ClosedAt
            };
        }
        
        #endregion
    
    }
}