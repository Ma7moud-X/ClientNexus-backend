//namespace ClientNexus.API.Controllers
//{
//    using ClientNexus.Application.Services;
//    using Microsoft.AspNetCore.Identity.Data;
//    using Microsoft.AspNetCore.Mvc;
//    using System.Threading.Tasks;

//    [ApiController]
//    [Route("api/[controller]")]
//    public class ForgotPasswordController : ControllerBase
//    {
//        private readonly ForgotPasswordService _forgotPasswordService;

//        public ForgotPasswordController(ForgotPasswordService forgotPasswordService)
//        {
//            _forgotPasswordService = forgotPasswordService;
//        }

//        [HttpPost("send-otp")]
//        public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordRequest request)
//        {
//            if (string.IsNullOrEmpty(request.Email))
//            {
//                return BadRequest("Email is required.");
//            }

//            try
//            {
//                await _forgotPasswordService.SendOtpAsync(request.Email);
//                return Ok("OTP sent successfully.");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}

