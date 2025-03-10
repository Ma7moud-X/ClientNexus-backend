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

public class AuthService : IAuthService
{
    private readonly UserManager<BaseUser> _userManager;
    private readonly SignInManager<BaseUser> _signInManager;
    private readonly IConfiguration _configuration;

    private static readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();



    public AuthService(UserManager<BaseUser> userManager, SignInManager<BaseUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;

    }

    public async Task<BaseUser> RegisterAsync(RegisterUserDTO registerDto)
    {
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
                Description = registerDto.Description ?? string.Empty,
                MainImage = registerDto.MainImage ?? string.Empty,
                TypeId = registerDto.TypeId,
                FirstName = registerDto.FirstName,  // NEW
                LastName = registerDto.LastName     // NEW
            },

            UserType.Client => new Client
            {
                UserType = UserType.Client,
                BirthDate = registerDto.BirthDate,
                FirstName = registerDto.FirstName,  // NEW
                LastName = registerDto.LastName     // NEW
            },

            _ => throw new Exception("Invalid UserType")
        };

        user.UserName = registerDto.Email;
        user.Email = registerDto.Email;

        var createResult = await _userManager.CreateAsync(user, registerDto.Password);

        if (!createResult.Succeeded)
            throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        return user;
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
