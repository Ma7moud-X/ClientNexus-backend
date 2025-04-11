using System.ComponentModel.DataAnnotations;
using ClientNexus.Application.Enums;

namespace ClientNexus.Application.DTO;

public class CreateOfferDTO
{
    [Required]
    public required int ServiceId { get; init; }

    [Required]
    public required double Price { get; init; }

    [Required]
    [Range(-180, 180)]
    public required double Longitude { get; init; }

    [Required]
    [Range(-90, 90)]
    public required double Latitude { get; init; }

    [Required]
    public required TravelProfile TransportationType { get; init; }
}
