namespace ClientNexus.Application.Services
{
    using ClientNexus.Application.DTOs;
    using ClientNexus.Application.Interfaces;
    using ClientNexus.Domain.Entities.Users;
    using ClientNexus.Domain.Enums;
    using ClientNexus.Domain.Interfaces;
    using Microsoft.AspNetCore.Identity;

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<BaseUser> _userManager;
        private readonly SignInManager<BaseUser> _signInManager;

        public AuthService(
            UserManager<BaseUser> userManager,
            SignInManager<BaseUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseUser?> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            return result.Succeeded ? user : null;
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


    }
}
