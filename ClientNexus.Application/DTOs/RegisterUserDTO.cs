using System.Text.Json.Serialization;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.DTOs
{
    public class RegisterUserDTO
    {
        public string FirstName { get; set; } = default!; // NEW - First Name Field
        public string LastName { get; set; } = default!; // NEW - Last Name Field
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;

        [JsonConverter(typeof(JsonStringEnumConverter))] // NEW - Enables JSON string to Enum conversion
        public UserType UserType { get; set; }

        public DateOnly BirthDate { get; set; }
        public int? AccessLevelId { get; set; } // For Admin
        public string? Description { get; set; } // For ServiceProvider
        public string? MainImage { get; set; } // For ServiceProvider
        public int TypeId { get; set; } // For ServiceProvider
    }

}