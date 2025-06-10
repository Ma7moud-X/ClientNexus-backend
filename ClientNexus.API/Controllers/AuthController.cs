using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace ClientNexus.API.Controllers
{
 

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        


        public AuthController(IAuthService authService, IConfiguration config)
        {
            this._authService = authService;
            _config = config;
           

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var authResponse = await _authService.LoginAsync(loginDto);

            if (authResponse == null)
                return Unauthorized("Invalid email or password.");

            return Ok(authResponse);
        }

        [HttpPost("SocialLogin")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginRequestDTO request)
        {
            try
            {
                var result = await _authService.SocialLogin(request);
                if (result == null)
                {
                    return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse("Authentication failed."));
                }

                return Ok(ApiResponseDTO<AuthResponseDTO>.SuccessResponse(result, "Social Login successful."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDTO<string>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<AuthResponseDTO>.ErrorResponse("An unexpected error occurred: " + ex.Message));
            }

        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserDTO dto) // NEW - Explicitly binding from body
        {


            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDTO<string>.ErrorResponse("Invalid request data"));

            try
            {
                var response = await _authService.RegisterAsync(dto);

                if (response == null)
                    return BadRequest(ApiResponseDTO<string>.ErrorResponse("Registration failed. Please try again."));

                return CreatedAtAction(nameof(Login), new { email = response.Email },
                    ApiResponseDTO<AuthResponseDTO>.SuccessResponse(response, "User registered successfully."));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
            }

        }
        [HttpPost("Socialregister")]
        public async Task<IActionResult> SocialRegister([FromForm] SocialAuthDTO dto) // NEW - Explicitly binding from body
        {


            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDTO<string>.ErrorResponse("Invalid request data"));

            try
            {
                var response = await _authService.SocialRegisterAsync(dto);

                if (response == null)
                    return BadRequest(ApiResponseDTO<string>.ErrorResponse("Registration failed. Please try again."));

                return CreatedAtAction(nameof(Login), new { email = response.Email },
                    ApiResponseDTO<AuthResponseDTO>.SuccessResponse(response, "User registered successfully."));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponseDTO<AuthResponseDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDTO<string>.ErrorResponse($"An error occurred: {ex.Message}"));
            }

        }

        [HttpGet("verify-token")]
        [AllowAnonymous]
        public IActionResult VerifyToken()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(ApiResponseDTO<string>.SuccessResponse("Token is valid"));
            }

            return Unauthorized(ApiResponseDTO<string>.ErrorResponse("Invalid or expired token"));
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