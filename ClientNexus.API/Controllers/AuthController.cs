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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.IdentityModel.Tokens.Jwt;

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
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var authResponse = await _authService.LoginAsync(loginDto);

            if (authResponse == null)
                return Unauthorized("Invalid email or password.");

            return Ok(authResponse);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto) // NEW - Explicitly binding from body
        {
            if (!ModelState.IsValid) // NEW - Proper validation handling
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(Login), new { email = user.Email }, user);
        }



       
        [HttpPost("logout")]
        public async Task<IActionResult> SignOut([FromHeader] string authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                return BadRequest("Invalid token format.");

            var token = authorization.Substring(7); // Remove "Bearer " prefix

            var result = await _authService.SignOutAsync(token);
            if (!result)
                return BadRequest("Logout failed.");

            return Ok("User successfully signed out.");
        }

    }
}