namespace ClientNexus.Application.Interfaces
{
    using ClientNexus.Application.DTOs;
    using ClientNexus.Domain.Entities.Users;

    public interface IAuthService
    {

        
            public Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto);
            public Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO registerDto);

            public Task<bool> SignOutAsync(string token); // UPDATED: Accepts token for revocation


    }

}