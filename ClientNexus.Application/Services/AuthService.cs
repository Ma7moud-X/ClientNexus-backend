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

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<BaseUser> _userManager;
    private readonly SignInManager<BaseUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IAddressService _addressService;
    private readonly IPhoneNumberService _phoneNumberService;
    private readonly ISpecializationService _specializationService;

    private static readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();



    public AuthService(UserManager<BaseUser> userManager, SignInManager<BaseUser> signInManager, IConfiguration configuration, IUnitOfWork unitOfWork, IAddressService addressService, IPhoneNumberService phoneNumberService, ISpecializationService specializationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _addressService = addressService;
        _phoneNumberService = phoneNumberService;
        _specializationService = specializationService;
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO registerDto)
    {
        if (registerDto == null)
        {
            throw new ArgumentNullException(nameof(registerDto), "Registration data cannot be null.");
        }

        BaseUser user = registerDto.UserType switch
        {
            UserType.Admin => new Admin
            {
                UserType = UserType.Admin,
                AccessLevelId = registerDto.AccessLevelId ?? throw new ArgumentNullException("AccessLevelId is required for Admin"),
                BirthDate = registerDto.BirthDate,
                FirstName = registerDto.FirstName,  // NEW
                LastName = registerDto.LastName     // NEW
            },

            UserType.ServiceProvider => new ServiceProvider
            {
                UserType = UserType.ServiceProvider,
                BirthDate = registerDto.BirthDate,
                Description = registerDto.Description ?? throw new ArgumentNullException(nameof(registerDto.Description), "Description is required for ServiceProvider"),
                ImageIDUrl = registerDto.ImageIDUrl ?? throw new ArgumentNullException(nameof(registerDto.ImageIDUrl), "ImageIDUrl is required for ServiceProvider"),
                ImageNationalIDUrl = registerDto.ImageNationalIDUrl ?? throw new ArgumentNullException(nameof(registerDto.ImageNationalIDUrl), "ImageNationalIDUrl is required for ServiceProvider"),
                MainImage = registerDto.MainImage ?? throw new ArgumentNullException(nameof(registerDto.MainImage), "MainImage is required for ServiceProvider"),
                TypeId = registerDto.TypeId ?? throw new ArgumentNullException(nameof(registerDto.TypeId), "TypeId is required for ServiceProvider"),
                FirstName = registerDto.FirstName,  // NEW
                LastName = registerDto.LastName,    // NEW
                Rate = 0,
                IsApproved = false,
                YearsOfExperience = registerDto.YearsOfExperience ?? throw new ArgumentNullException(nameof(registerDto.YearsOfExperience), "YearsOfExperience is required for ServiceProvider"),
            },

            UserType.Client => new Client
            {
                UserType = UserType.Client,
                BirthDate = registerDto.BirthDate,
                FirstName = registerDto.FirstName,  // NEW
                LastName = registerDto.LastName,    // NEW
                Rate = 0
            },

            _ => throw new Exception("Invalid UserType")
        };

        user.UserName = registerDto.Email;
        user.Email = registerDto.Email;
        user.PhoneNumber = registerDto.PhoneNumber;





        var createResult = await _userManager.CreateAsync(user, registerDto.Password);

        if (!createResult.Succeeded)
        {
            string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        if (!(registerDto.PhoneNumbers == null || !registerDto.PhoneNumbers.Any()))
        {
            user.PhoneNumbers = new List<PhoneNumber>();
            await _phoneNumberService.AddCollectionOfPhoneNumer(user.PhoneNumbers, registerDto.PhoneNumbers);
        }

        if (user is ServiceProvider serviceProvider)
        {
            serviceProvider.ServiceProviderSpecializations = new List<ServiceProviderSpecialization>();
            await _specializationService.AddSpecializationsToServiceProvider(serviceProvider.ServiceProviderSpecializations, registerDto.SpecializationIDS, serviceProvider.Id);


            // Add multiple addresses using AddressService
            if (registerDto.Addresses == null || !registerDto.Addresses.Any())
            {

                throw new ArgumentNullException("Addresses are required for ServiceProvider.");
            }

            foreach (var addressDto in registerDto.Addresses)
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
}