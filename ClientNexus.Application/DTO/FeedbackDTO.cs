using System;
using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO
{
    public class FeedbackDTO
    {
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public int ServiceProviderId { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public float Rate { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Feedback comment cannot exceed 1000 characters")]
        public string? Feedback { get; set; }
    }
    
    public class UpdateFeedbackDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public float Rate { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Feedback comment cannot exceed 1000 characters")]
        public string? Feedback { get; set; }
    }
   
}