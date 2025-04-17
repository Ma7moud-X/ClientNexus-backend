using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly IOtpService _otpService;

        public PasswordResetController(IPasswordResetService passwordResetService, IOtpService otpService)
        {
            _passwordResetService = passwordResetService;
            _otpService = otpService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDTO request)
        {
            try
            {
                var otp = await _otpService.GenerateOtpAsync(request.Email); // Call GenerateOtpAsync
                await _otpService.SendOtpAsync(request.Email, otp); // Send OTP via email
                return Ok("OTP sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending OTP: {ex.Message}");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequestDTO request)
        {
            try
            {
                await _passwordResetService.ResetPasswordAsync(request); // Reset password logic
                return Ok("Password reset successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting password: {ex.Message}");
            }
        }
    }


}
