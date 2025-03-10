using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO;

public class EmergencyCaseDTO
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Description { get; init; }
}
