using System.ComponentModel.DataAnnotations;
using ClientNexus.Application.Enums;

namespace ClientNexus.Application.DTO;

public class CreateOfferDTO
{
    [Required]
    public required decimal Price { get; init; }

    [Required]
    public required TravelProfile TransportationType { get; init; }
}
