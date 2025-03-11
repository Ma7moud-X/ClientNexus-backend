using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO;

public class CreateEmergencyCaseDTO
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Description { get; init; }

    [Required]
    public required double MeetingLatitude { get; init; }

    [Required]
    public required double MeetingLongitude { get; init; }
}
