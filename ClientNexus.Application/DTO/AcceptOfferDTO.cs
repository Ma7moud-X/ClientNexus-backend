using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO;

public class AcceptOfferDTO
{
    [Required]
    public required int ServiceProviderId { get; init; }
}
