using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces
{
    public interface IFeedbackService
    {
        // Get feedback by ID
        Task<FeedbackDTO> GetByIdAsync(int FeedbackId);
        
        // Get all feedback for a specific service provider with pagination
        Task<IEnumerable<FeedbackDTO>> GetAllForServiceProviderAsync(int serviceProviderId, int pageNumber = 1, int pageSize = 10);
        
        // Get all feedback given by a specific client with pagination
        Task<IEnumerable<FeedbackDTO>> GetAllByClientAsync(int clientId, int pageNumber = 1, int pageSize = 10);
        
        // Get average rating for a service provider
        Task<float> GetAverageRatingForServiceProviderAsync(int serviceProviderId);
        
        // Create new feedback
        Task<FeedbackDTO> CreateClientToProviderFeedbackAsync(CreateFeedbackDTO createFeedbackDto);
        
        // Update existing feedback
        Task<FeedbackDTO> UpdateFeedbackAsync(UpdateFeedbackDTO updateFeedbackDto, int FeedbackId);
        
        // Delete feedback
        Task<bool> DeleteFeedbackAsync(int FeedbackId);

        // Get total feedback count
        int GetTotalFeedbackCount(int? clientId = null, int? serviceProviderId = null);
    }
}