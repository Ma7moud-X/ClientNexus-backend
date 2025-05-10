using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Entities.Others;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Azure.Core;

public class AuthService : IAuthService
{
    private readonly IFileService fileService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<BaseUser> _userManager;
    private readonly SignInManager<BaseUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IAddressService _addressService;
    private readonly IPhoneNumberService _phoneNumberService;
    private readonly ISpecializationService _specializationService;
 

    private static readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();



    public AuthService(UserManager<BaseUser> userManager, SignInManager<BaseUser> signInManager, IConfiguration configuration, IUnitOfWork unitOfWork, IAddressService addressService, IPhoneNumberService phoneNumberService, ISpecializationService specializationService, IFileService fileService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _addressService = addressService;
        _phoneNumberService = phoneNumberService;
        _specializationService = specializationService;
        this.fileService = fileService;
       
    }


    public async Task<AuthResponseDTO> SocialRegisterAsync(SocialAuthDTO dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Registration data cannot be null.");
        }
        if (!Enum.IsDefined(typeof(UserType), dto.UserType) || dto.UserType == 0)
        {
            throw new ArgumentException("UserType is required and must be valid.");
        }
        if (!Enum.IsDefined(typeof(Gender), dto.Gender) || dto.Gender == 0)
        {
            throw new ArgumentException("Gender is required and must be valid.");
        }

        SocialUserPayload payload;
        payload = await GetSocialPayloadAsync(dto.Provider, dto.AccessToken);

        var existingUser = await _userManager.FindByEmailAsync(payload.Email);

        if (existingUser != null)
            throw new Exception("User with this account already exists.");
        var mainImageUrl = string.Empty;
        var imageIDUrl = string.Empty;
        var imageNationalIDUrl = string.Empty;


        if (dto.UserType == UserType.ServiceProvider || dto.UserType == UserType.Client)
        {
            if (dto.MainImage == null && dto.UserType == UserType.ServiceProvider)
            {
                throw new ArgumentNullException(nameof(dto.MainImage), "MainImage is required for ServiceProvider.");
            }
            else if (dto.MainImage == null && dto.UserType == UserType.Client)
            {
                mainImageUrl = null;
            }
            else
            {
                mainImageUrl = await UploadImageAsync(dto.MainImage);

            }
        }

        if (dto.UserType == UserType.ServiceProvider)
        {

            if (dto.Addresses == null || !dto.Addresses.Any())
            {

                throw new ArgumentNullException("Addresses are required for ServiceProvider.");
            }

            if (dto.ImageIDUrl == null)
            {
                throw new ArgumentNullException(nameof(dto.ImageIDUrl), "ImageIDUrl is required for ServiceProvider.");
            }
            else
            {
                // Generate a custom key for the ID image with Guid and extension
                imageIDUrl = await UploadImageAsync(dto.ImageIDUrl);

            }

            if (dto.ImageNationalIDUrl == null)
            {
                throw new ArgumentNullException(nameof(dto.ImageNationalIDUrl), "ImageNationalIDUrl is required for ServiceProvider.");
            }
            else
            {
                // Generate a custom key for the National ID image with Guid and extension
                
                imageNationalIDUrl = await UploadImageAsync(dto.ImageNationalIDUrl);
            }

        }


        BaseUser user = dto.UserType switch
        {
            //UserType.Admin => new Admin
            //{
            //    UserType = UserType.Admin,
            //    AccessLevelId = dto.AccessLevelId ?? throw new ArgumentNullException("AccessLevelId is required for Admin"),

            //},

            UserType.ServiceProvider => new ServiceProvider
            {
                UserType = UserType.ServiceProvider,
                Description = dto.Description ?? throw new ArgumentNullException(nameof(dto.Description), "Description is required for ServiceProvider"),
                ImageIDUrl = imageIDUrl,
                ImageNationalIDUrl = imageNationalIDUrl,
                MainImage = mainImageUrl,
                TypeId = dto.TypeId ?? throw new ArgumentNullException(nameof(dto.TypeId), "TypeId is required for ServiceProvider"),
                Rate = 0,
                IsApproved = false,
                YearsOfExperience = dto.YearsOfExperience ?? throw new ArgumentNullException(nameof(dto.YearsOfExperience), "YearsOfExperience is required for ServiceProvider"),
                Office_consultation_price = dto.Office_consultation_price ?? throw new ArgumentNullException(nameof(dto.TypeId), "Office consultation price is required for ServiceProvider"),
                Telephone_consultation_price = dto.Telephone_consultation_price ?? throw new ArgumentNullException(nameof(dto.TypeId), "Telephone consultation price is required for ServiceProvider"),
                main_specializationID = dto.main_specializationID ?? throw new ArgumentNullException(nameof(dto.TypeId), "main_specialization consultation price is required for ServiceProvider"),
            },

            UserType.Client => new Client
            {
                UserType = UserType.Client,
            },

            _ => throw new Exception("Invalid UserType")
        };
        user.BirthDate = dto.BirthDate;
        user.FirstName = payload.FirstName;// NEW
        user.LastName = payload.LastName;  // NEW
        user.Gender = dto.Gender;
        user.UserName = payload.Email;
        user.Email = payload.Email;
        user.PhoneNumber = dto.PhoneNumber;



        var password = Guid.NewGuid().ToString(); 

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        if (dto.PhoneNumbers != null && dto.PhoneNumbers.Any(x => !string.IsNullOrWhiteSpace(x)))
        {
            user.PhoneNumbers = new List<PhoneNumber>();
            await _phoneNumberService.AddCollectionOfPhoneNumer(user.PhoneNumbers, dto.PhoneNumbers);
        }

        if (user is ServiceProvider serviceProvider)
        {
            if (!((dto.SpecializationIDS) == null || !dto.SpecializationIDS.Any()))
            {
                serviceProvider.ServiceProviderSpecializations = new List<ServiceProviderSpecialization>();
                await _specializationService.AddSpecializationsToServiceProvider(serviceProvider.ServiceProviderSpecializations, dto.SpecializationIDS, serviceProvider.Id);

            }



            // Add multiple addresses using AddressService


            foreach (var addressDto in dto.Addresses)
            {
                await _addressService.AddAddressAsync(serviceProvider.Id, addressDto);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDTO
        {
            Token = token,
            Email = user.Email!,
            UserType = user.UserType.ToString()
        };


    }



    public async Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO dto )
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Registration data cannot be null.");
        }
        if (!Enum.IsDefined(typeof(UserType), dto.UserType) || dto.UserType == 0)
        {
            throw new ArgumentException("UserType is required and must be valid.");
        }
        if (!Enum.IsDefined(typeof(Gender), dto.Gender) || dto.Gender == 0)
        {
            throw new ArgumentException("Gender is required and must be valid.");
        }
        var mainImageUrl = string.Empty;
        var imageIDUrl = string.Empty;
        var imageNationalIDUrl = string.Empty;


        if (dto.UserType == UserType.ServiceProvider || dto.UserType == UserType.Client)
        {
            if (dto.MainImage == null && dto.UserType == UserType.ServiceProvider)
            {
                throw new ArgumentNullException(nameof(dto.MainImage), "MainImage is required for ServiceProvider.");
            }
            else if(dto.MainImage == null && dto.UserType == UserType.Client)
            {
                mainImageUrl = null;
            }
            else
            {
                mainImageUrl = await UploadImageAsync(dto.MainImage);

            }
        }          

            if (dto.UserType == UserType.ServiceProvider)
        {
            
            if (dto.Addresses == null || !dto.Addresses.Any())
            {

                throw new ArgumentNullException("Addresses are required for ServiceProvider.");
            }

            if (dto.ImageIDUrl == null)
            {
                throw new ArgumentNullException(nameof(dto.ImageIDUrl), "ImageIDUrl is required for ServiceProvider.");
            }
            else
            {
                // Generate a custom key for the ID image with Guid and extension
                imageIDUrl = await UploadImageAsync(dto.ImageIDUrl);

            }

            if (dto.ImageNationalIDUrl == null)
            {
                throw new ArgumentNullException(nameof(dto.ImageNationalIDUrl), "ImageNationalIDUrl is required for ServiceProvider.");
            }
            else
            {
                // Generate a custom key for the National ID image with Guid and extension

                imageNationalIDUrl = await UploadImageAsync(dto.ImageNationalIDUrl);
            }

        }


        BaseUser user = dto.UserType switch
        {
            UserType.Admin => new Admin
            {
                UserType = UserType.Admin,
                AccessLevelId = dto.AccessLevelId ?? throw new ArgumentNullException("AccessLevelId is required for Admin"),

            },

            UserType.ServiceProvider => new ServiceProvider
            {

                UserType = UserType.ServiceProvider,
                Description = dto.Description ?? throw new ArgumentNullException(nameof(dto.Description), "Description is required for ServiceProvider"),
                ImageIDUrl = imageIDUrl,
                ImageNationalIDUrl = imageNationalIDUrl,
                MainImage = mainImageUrl,
                TypeId = dto.TypeId ?? throw new ArgumentNullException(nameof(dto.TypeId), "TypeId is required for ServiceProvider"),
                Rate = 0,
                IsApproved = false,
                YearsOfExperience = dto.YearsOfExperience ?? throw new ArgumentNullException(nameof(dto.YearsOfExperience), "YearsOfExperience is required for ServiceProvider"),
                Office_consultation_price = dto.Office_consultation_price ?? throw new ArgumentNullException(nameof(dto.TypeId), "Office consultation price is required for ServiceProvider"),
                Telephone_consultation_price = dto.Telephone_consultation_price ?? throw new ArgumentNullException(nameof(dto.TypeId), "Telephone consultation price is required for ServiceProvider"),
                main_specializationID = dto.main_specializationID ?? throw new ArgumentNullException(nameof(dto.TypeId), "main_specialization consultation price is required for ServiceProvider"),
            },

            UserType.Client => new Client
            {
                UserType = UserType.Client,
            },

            _ => throw new Exception("Invalid UserType")
        };
        user.BirthDate = dto.BirthDate;
        user.FirstName = dto.FirstName;// NEW
        user.LastName = dto.LastName;  // NEW
        user.Gender = dto.Gender;
        user.UserName = dto.Email;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
       


        var createResult = await _userManager.CreateAsync(user, dto.Password);

        if (!createResult.Succeeded)
        {
            string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        if (dto.PhoneNumbers != null && dto.PhoneNumbers.Any(x => !string.IsNullOrWhiteSpace(x)))
        {
            user.PhoneNumbers = new List<PhoneNumber>();
            await _phoneNumberService.AddCollectionOfPhoneNumer(user.PhoneNumbers, dto.PhoneNumbers);
        }

        if (user is ServiceProvider serviceProvider)
        {
            if (!((dto.SpecializationIDS) == null || !dto.SpecializationIDS.Any()))
            {
                serviceProvider.ServiceProviderSpecializations = new List<ServiceProviderSpecialization>();
                await _specializationService.AddSpecializationsToServiceProvider(serviceProvider.ServiceProviderSpecializations, dto.SpecializationIDS, serviceProvider.Id);

            }



            // Add multiple addresses using AddressService


            foreach (var addressDto in dto.Addresses)
            {
                await _addressService.AddAddressAsync(serviceProvider.Id, addressDto);
            }
        }
      
        await _unitOfWork.SaveChangesAsync();
       
        var token = GenerateJwtToken(user);

        return new AuthResponseDTO
        {
            Token = token,
            Email = user.Email!,
            UserType = user.UserType.ToString()
        };



    }




    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return null;

        // Generate JWT Token
        var token = GenerateJwtToken(user);

        return new AuthResponseDTO
        {
            Token = token,
            Email = user.Email!,
            UserType = user.UserType.ToString()
        };
    }
    public async  Task<AuthResponseDTO?> SocialLogin(SocialLoginRequestDTO request)
    {
        if (request == null )
        {
            throw new ArgumentException("Invalid login request.");
        }
        SocialUserPayload payload;

        payload = await GetSocialPayloadAsync(request.Provider, request.AccessToken);

        var user = await _userManager.FindByEmailAsync(payload.Email);
        if (user == null)
            throw new InvalidOperationException("User not  exists in database");



        var token = GenerateJwtToken(user);
        return new AuthResponseDTO
        {
            Token = token,
            Email = user.Email!,
            UserType = user.UserType.ToString()
        };
    }
    private string GenerateJwtToken(BaseUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.UserType.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    public async Task<bool> SignOutAsync(string token) // UPDATED: Accepts token for revocation
    {
        if (string.IsNullOrEmpty(token)) return false;

        // Add token to revoked list
        _revokedTokens[token] = DateTime.UtcNow.AddHours(2); // Store expiration time

        await _signInManager.SignOutAsync();
        return true;
    }

    public static bool IsTokenRevoked(string token)
    {
        return _revokedTokens.ContainsKey(token);
    }
    private async Task<SocialUserPayload> ValidateFacebookTokenAsync(string accessToken)
    {
        using var http = new HttpClient();
        var fbUrl = $"https://graph.facebook.com/me?fields=id,first_name,last_name,email,picture&access_token={accessToken}";

        var response = await http.GetStringAsync(fbUrl);
        var json = JsonDocument.Parse(response).RootElement;

        return new SocialUserPayload
        {
            Email = json.GetProperty("email").GetString() ?? "",
            FirstName = json.GetProperty("first_name").GetString() ?? "",
            LastName = json.GetProperty("last_name").GetString() ?? "",
        };
    }
    private async Task<string> UploadImageAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).TrimStart('.');
        var key = $"{Guid.NewGuid()}.{extension}";
        var fileType = GetFileType(file);
        return await fileService.UploadPublicFileAsync(file.OpenReadStream(), fileType, key);
    }
    private async Task<SocialUserPayload> GetSocialPayloadAsync(string provider, string accessToken)
    {
        return provider.ToLower() switch
        {
            "google" => await GetGooglePayloadAsync(accessToken),
            "facebook" => await ValidateFacebookTokenAsync(accessToken),
            _ => throw new NotSupportedException("Unsupported provider.")
        };
    }

    private async Task<SocialUserPayload> GetGooglePayloadAsync(string accessToken)
    {
        var googlePayload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
        return new SocialUserPayload
        {
            Email = googlePayload.Email,
            FirstName = googlePayload.GivenName,
            LastName = googlePayload.FamilyName,
        };
    }



    private FileType GetFileType(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        switch (extension)
        {
            case ".jpg":
                return FileType.Jpg;
            case ".jpeg":
                return FileType.Jpeg;
            case ".png":
                return FileType.Png;
            default:
                throw new ArgumentException($"Unsupported file type: {extension}");
        }
    }
}