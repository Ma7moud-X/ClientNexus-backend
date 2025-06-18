using ClientNexus.Application.DTOs;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> StartSubscriptionPayment([FromBody] StartSubscriptionPaymentRequestDTO request)
        {
            var response = await _paymentService.StartSubscriptionPayment(request);
            return Ok(response);
        }

        [HttpPost("service")]
        public async Task<IActionResult> StartServicePayment([FromBody] StartServicePaymentRequestDTO request)
        {
            var response = await _paymentService.StartServicePayment(request);
            return Ok(response);
        }

        [HttpGet("verify/{intentionId}")]
        public async Task<IActionResult> VerifyPayment(string intentionId)
        {
            var response = await _paymentService.VerifyPayment(intentionId);
            return Ok(response);
        }
        [HttpGet("status/{referenceNumber}")]
        public async Task<IActionResult> GetPaymentStatus(string referenceNumber)
        {
            var response = await _paymentService.GetPaymentStatus(referenceNumber);
            return Ok(response);
        }

    }

}
