using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ClientNexus.Domain.Entities.Others;
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
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Please enter an 11-digit phone number.")]
        public string PhoneNumber { get; set; }
        [PhoneListValidation]
        public List<string>? PhoneNumbers { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))] // NEW - Enables JSON string to Enum conversion
        [Required]
        public UserType UserType { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public int? AccessLevelId { get; set; } // For Admin
        public string? Description { get; set; } // For ServiceProvider
        public IFormFile? MainImage { get; set; } // For ServiceProvider and client
        public IFormFile? ImageIDUrl { get; set; } // For ServiceProvider
        public IFormFile? ImageNationalIDUrl { get; set; } // For ServiceProvider
        public int? YearsOfExperience { get; set; }// For ServiceProvider
        public List<int>? SpecializationIDS { get; set; }// For ServiceProvider
        public int? TypeId { get; set; } // For ServiceProvider
      
        public List<AddressDTO>? Addresses { get; set; }  // For ServiceProvider
        public int? Office_consultation_price { get; set; } // For ServiceProvider
        public int? Telephone_consultation_price { get; set; } // For ServiceProvider
        public int? main_specializationID { get; set; }// For ServiceProvider


    }

}