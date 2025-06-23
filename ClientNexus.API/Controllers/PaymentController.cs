using ClientNexus.Application.DTOs;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientNexus.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("subscription")]
        [Authorize(Policy = "IsServiceProvider")] // Restrict to ServiceProvider role
        public async Task<IActionResult> StartSubscriptionPayment([FromBody] StartSubscriptionPaymentRequestDTO request)
        {
            // Extract ServiceProviderId from token
            var serviceProviderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(serviceProviderIdClaim) || !int.TryParse(serviceProviderIdClaim, out int serviceProviderId))
            {
                return Unauthorized("Invalid or missing ServiceProviderId in token.");
            }

            var response = await _paymentService.StartSubscriptionPayment(request, serviceProviderId);
            return Ok(response);
        }

        [HttpPost("service")]
        [Authorize(Policy = "IsClient")]     // Requires logged-in user
        public async Task<IActionResult> StartServicePayment([FromBody] StartServicePaymentRequestDTO request)
        {
            // Extract ClientId from token
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
            {
                return Unauthorized("Invalid or missing ClientId in token.");
            }

            var response = await _paymentService.StartServicePayment(request, clientId);
            return Ok(response);
        }

        [HttpGet("verify/{intentionId}")]
        [Authorize] // Requires logged-in user
        public async Task<IActionResult> VerifyPayment(string intentionId)
        {
            var response = await _paymentService.VerifyPayment(intentionId);
            return Ok(response);
        }

        [HttpGet("status/{referenceNumber}")]
        [Authorize] // Requires logged-in user
        public async Task<IActionResult> GetPaymentStatus(string referenceNumber)
        {
            var response = await _paymentService.GetPaymentStatus(referenceNumber);
            return Ok(response);
        }
    }

}
