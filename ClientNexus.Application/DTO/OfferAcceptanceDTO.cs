using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO;

public class OfferAcceptanceDTO
{
    [Required]
    public required int ServiceProviderId { get; init; }
}
