namespace ClientNexus.Application.Interfaces
{
    using ClientNexus.Application.DTOs;
    using ClientNexus.Domain.Entities.Users;

    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto); // UPDATED - Return token response

        Task<BaseUser> RegisterAsync(RegisterUserDTO registerDto);
        Task<bool> SignOutAsync(string token); // UPDATED: Accepts JWT token for revocation


    }

}