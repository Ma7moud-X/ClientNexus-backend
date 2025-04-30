using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ClientNexus.Application.DTOs
{
    public class RegisterUserDTO
    {
        [Required]
        public string FirstName { get; set; } // NEW - First Name Field
        [Required]
        public string LastName { get; set; }  // NEW - Last Name Field
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public DateOnly BirthDate { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public List<string>? PhoneNumbers { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))] // NEW - Enables JSON string to Enum conversion
        [Required]
        public UserType UserType { get; set; }
        public int? AccessLevelId { get; set; } // For Admin
        public string? Description { get; set; } // For ServiceProvider
        public IFormFile? MainImage { get; set; } // For ServiceProvider
        public IFormFile? ImageIDUrl { get; set; } // For ServiceProvider
        public IFormFile? ImageNationalIDUrl { get; set; } // For ServiceProvider
        //public string? MainImage { get; set; } // For ServiceProvider
        //public string? ImageIDUrl { get; set; } // For ServiceProvider
        //public string? ImageNationalIDUrl { get; set; } // For ServiceProvider
        public int? YearsOfExperience { get; set; }// For ServiceProvider
        public List<int>? SpecializationIDS { get; set; }// For ServiceProvider
        public int? TypeId { get; set; } // For ServiceProvider
        public List<AddressDTO>? Addresses { get; set; } // For ServiceProvider
        public int? Office_consultation_price { get; set; } // For ServiceProvider
        public int? Telephone_consultation_price { get; set; } // For ServiceProvider
        public int? main_specializationID { get; set; }// For ServiceProvider

    }

}