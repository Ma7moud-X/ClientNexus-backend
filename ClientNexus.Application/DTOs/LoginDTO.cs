
namespace ClientNexus.Application.DTOs
{
    using ClientNexus.Domain.Enums;

   
        public class LoginDTO
        {
            public string Email { get; set; } = default!;
            public string Password { get; set; } = default!;
        }
    
}