using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClientNexus.API.Controllers
{
    using ClientNexus.Application.DTOs;
    using ClientNexus.Application.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto) // NEW - Explicitly binding from body
        {
            var user = await _authService.LoginAsync(loginDto);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto) // NEW - Explicitly binding from body
        {
            if (!ModelState.IsValid) // NEW - Proper validation handling
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(Login), new { email = user.Email }, user);
        }
    }
}