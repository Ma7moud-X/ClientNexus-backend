using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<FeedbackDTO> GetByIdAsync(int FeedbackId)
        {
            var feedback = await _unitOfWork.ClientServiceProviderFeedbacks.GetByIdAsync(FeedbackId)
            ?? throw new KeyNotFoundException("Invalid Feedback ID");
            
            return MapToDto(feedback);
        }
        
        public async Task<IEnumerable<FeedbackDTO>> GetAllForServiceProviderAsync(int serviceProviderId)
        {
            var providerExists = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId) 
            ?? throw new KeyNotFoundException($"Service provider with ID {serviceProviderId} not found");

            var feedbacks = await _unitOfWork.ClientServiceProviderFeedbacks.GetByConditionAsync(f => f.ServiceProviderId == serviceProviderId);
                
            return feedbacks.Select(MapToDto);
        }
        
        public async Task<IEnumerable<FeedbackDTO>> GetAllByClientAsync(int clientId)
        {
            var clientExists = await _unitOfWork.Clients.GetByIdAsync(clientId) 
            ?? throw new KeyNotFoundException($"Client with ID {clientId} not found");

            var feedbacks = await _unitOfWork.ClientServiceProviderFeedbacks.GetByConditionAsync(f => f.ClientId == clientId);
                
            return feedbacks.Select(MapToDto);
        }
        
        public async Task<float> GetAverageRatingForServiceProviderAsync(int serviceProviderId)
        {
            var providerExists = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId) 
            ?? throw new KeyNotFoundException($"Service provider with ID {serviceProviderId} not found");

            var feedbacks = await _unitOfWork.ClientServiceProviderFeedbacks.GetByConditionAsync(f => f.ServiceProviderId == serviceProviderId);
                
            if (!feedbacks.Any())
                return 0;
                
            return feedbacks.Average(f => f.Rate);
        }
        
        public async Task<FeedbackDTO> CreateClientToProviderFeedbackAsync(CreateFeedbackDTO createFeedbackDto)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(createFeedbackDto.ClientId) 
            ?? throw new KeyNotFoundException($"Client with ID {createFeedbackDto.ClientId} not found");

            var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(createFeedbackDto.ServiceProviderId) 
            ?? throw new KeyNotFoundException($"Service provider with ID {createFeedbackDto.ServiceProviderId} not found");

            var feedback = new ClientServiceProviderFeedback
            {
                ClientId = createFeedbackDto.ClientId,
                ServiceProviderId = createFeedbackDto.ServiceProviderId,
                Rate = createFeedbackDto.Rate,
                Feedback = createFeedbackDto.Feedback,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.ClientServiceProviderFeedbacks.AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            
            await UpdateServiceProviderRating(createFeedbackDto.ServiceProviderId);
            
            return MapToDto(feedback);
        }
        
        public async Task<FeedbackDTO> UpdateFeedbackAsync(UpdateFeedbackDTO updateFeedbackDto, int FeedbackId)
        {
            var feedback = await _unitOfWork.ClientServiceProviderFeedbacks.GetByIdAsync(FeedbackId)
            ?? throw new KeyNotFoundException("Feedback not found");
                
            feedback.Rate = updateFeedbackDto.Rate;
            feedback.Feedback = updateFeedbackDto.Feedback;
            
            await _unitOfWork.SaveChangesAsync();
            
            // Update service provider's average rating
            await UpdateServiceProviderRating(feedback.ServiceProviderId);
            
            return MapToDto(feedback);
        }
        
        public async Task<bool> DeleteFeedbackAsync(int FeedbackId)
        {
            var feedback = await _unitOfWork.ClientServiceProviderFeedbacks.GetByIdAsync(FeedbackId)
            ?? throw new KeyNotFoundException("Feedback not found");
                
            _unitOfWork.ClientServiceProviderFeedbacks.Delete(feedback);
            await _unitOfWork.SaveChangesAsync();
            
            // Update service provider's average rating
            await UpdateServiceProviderRating(feedback.ServiceProviderId);
            
            return true;
        }
        
        // Helper method to update service provider's average rating
        private async Task UpdateServiceProviderRating(int serviceProviderId)
        {
            var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId);
            if (serviceProvider == null)
                return;
            
            var feedbacks = await _unitOfWork.ClientServiceProviderFeedbacks.GetByConditionAsync(f => f.ServiceProviderId == serviceProviderId);
                
            if (feedbacks.Any())
            {
                serviceProvider.Rate = feedbacks.Average(f => f.Rate);
            }
            else
            {
                serviceProvider.Rate = 0;
            }
            
            await _unitOfWork.SaveChangesAsync();
        }
        
        // Helper method to map domain entity to DTO
        private FeedbackDTO MapToDto(ClientServiceProviderFeedback feedback)
        {
            return new FeedbackDTO
            {
                Id = feedback.Id,
                ClientId = feedback.ClientId,
                ServiceProviderId = feedback.ServiceProviderId,
                Rate = feedback.Rate,
                Feedback = feedback.Feedback
            };
        }
    }
}