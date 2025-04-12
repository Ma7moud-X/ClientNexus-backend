


using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces
{
    public interface IFeedbackService
    {
        // Get feedback by ID
        Task<FeedbackDTO> GetByIdAsync(int FeedbackId);
        
        // Get all feedback for a specific service provider
        Task<IEnumerable<FeedbackDTO>> GetAllForServiceProviderAsync(int serviceProviderId);
        
        // Get all feedback given by a specific client
        Task<IEnumerable<FeedbackDTO>> GetAllByClientAsync(int clientId);
        
        // Get average rating for a service provider
        Task<float> GetAverageRatingForServiceProviderAsync(int serviceProviderId);
        
        // Create new feedback
        Task<FeedbackDTO> CreateClientToProviderFeedbackAsync(CreateFeedbackDTO createFeedbackDto);
        
        // Update existing feedback
        Task<FeedbackDTO> UpdateFeedbackAsync(UpdateFeedbackDTO updateFeedbackDto, int FeedbackId);
        
        // Delete feedback
        Task<bool> DeleteFeedbackAsync(int FeedbackId);
    }
}