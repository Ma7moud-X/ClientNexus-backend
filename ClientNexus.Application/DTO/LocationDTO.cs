using System.ComponentModel.DataAnnotations;

namespace ClientNexus.Application.DTO
{
    public record LocationDTO
    {
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; init; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; init; }
    }
}