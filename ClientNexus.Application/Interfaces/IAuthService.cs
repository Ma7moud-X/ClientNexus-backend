namespace ClientNexus.Application.Interfaces
{
    using ClientNexus.Application.DTOs;
    using ClientNexus.Domain.Entities.Users;

    public interface IAuthService
    {
        Task<BaseUser?> LoginAsync(LoginDTO loginDto);
        Task<BaseUser> RegisterAsync(RegisterUserDTO registerDto);
    }
}