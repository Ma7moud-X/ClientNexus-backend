using Azure.Core;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClientNexus.API.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly string _hmacSecret;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<WebhookController> logger)
        {
            _unitOfWork = unitOfWork;
            _hmacSecret = configuration["Paymob:HmacSecret"];
            _logger = logger; // Inject ILogger for logging
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                // Log the start of the webhook request processing
                _logger.LogInformation("Webhook request processing started at {Time}", DateTime.UtcNow);

                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation("Received webhook body: {Body}", body); // Log the raw request body

                var data = JsonConvert.DeserializeObject<dynamic>(body);

                // Check hmac in query string if not in headers
                var receivedHmac = Request.Headers["hmac"].ToString();
                if (string.IsNullOrEmpty(receivedHmac))
                {
                    receivedHmac = Request.Query["hmac"].ToString();
                    _logger.LogInformation("HMAC not found in headers, found in query string: {Hmac}", receivedHmac);
                }
                else
                {
                    _logger.LogInformation("HMAC found in headers: {Hmac}", receivedHmac);
                }

                if (string.IsNullOrEmpty(receivedHmac) || !ValidateHmac(data, receivedHmac))
                {
                    _logger.LogWarning("HMAC validation failed. Received: {ReceivedHmac}, Computed: {ComputedHmac}", receivedHmac, ValidateHmac(data, receivedHmac) ? "Valid" : "Invalid");
                    return Unauthorized("Invalid HMAC signature");
                }
                _logger.LogInformation("HMAC validation succeeded");

                string type = data.type?.ToString();
                var obj = data.obj;
                string transactionId = obj?.id?.ToString();
                string success = obj?.success?.ToString()?.ToLower();

                _logger.LogInformation("Processing transaction with type: {Type}, ID: {TransactionId}, Success: {Success}", type, transactionId, success);

                if (type != "TRANSACTION" || string.IsNullOrEmpty(transactionId))
                {
                    _logger.LogWarning("Invalid webhook payload. Type: {Type}, TransactionId: {TransactionId}", type, transactionId);
                    return BadRequest("Invalid webhook payload");
                }

                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.IntentionId == transactionId);
                if (payment == null)
                {
                    _logger.LogWarning("Payment not found for IntentionId: {TransactionId}", transactionId);
                    return NotFound("Payment not found");
                }
                _logger.LogInformation("Found payment with ID: {PaymentId}", payment.Id);

                await _unitOfWork.BeginTransactionAsync();
                _logger.LogInformation("Transaction started for payment ID: {PaymentId}", payment.Id);

                payment.WebhookStatus = success == "true" ? "success" : "failed";
                payment.Status = success == "true" ? PaymentStatus.Completed : PaymentStatus.Failed;
                _logger.LogInformation("Updated payment status to WebhookStatus: {WebhookStatus}, Status: {Status}", payment.WebhookStatus, payment.Status);

                if (success == "true")
                {
                    if (payment.PaymentType == PaymentType.Subscription)
                    {
                        var subscriptionPayment = await _unitOfWork.SubscriptionPayments.FirstOrDefaultAsync(
                            sp => sp.Id == payment.Id,
                            q => q.Include(sp => sp.ServiceProvider)
                        );
                        if (subscriptionPayment != null)
                        {
                            _logger.LogInformation("Found subscription payment for ID: {PaymentId}", payment.Id);
                            var serviceProvider = subscriptionPayment.ServiceProvider;
                            serviceProvider.SubscriptionStatus = SubscriptionStatus.Active;
                            serviceProvider.SubscriptionExpiryDate = subscriptionPayment.SubscriptionType switch
                            {
                                'M' => DateTime.UtcNow.AddMonths(1),
                                'Q' => DateTime.UtcNow.AddMonths(3),
                                'Y' => DateTime.UtcNow.AddYears(1),
                                _ => DateTime.UtcNow.AddMonths(1)
                            };
                            serviceProvider.SubType = subscriptionPayment.SubscriptionTier switch
                            {
                                "Normal" => SubscriptionType.Basic,
                                "Advanced" => SubscriptionType.Premium,
                                _ => serviceProvider.SubType
                            };
                            _unitOfWork.ServiceProviders.Update(serviceProvider, serviceProvider);
                            _logger.LogInformation("Updated ServiceProvider with ID: {ServiceProviderId}", serviceProvider.Id);
                        }
                        else
                        {
                            _logger.LogWarning("SubscriptionPayment not found for Payment ID: {PaymentId}", payment.Id);
                        }
                    }
                    else if (payment.PaymentType == PaymentType.Service)
                    {
                        var servicePayment = await _unitOfWork.ServicePayments.FirstOrDefaultAsync(
                            sp => sp.Id == payment.Id,
                            q => q.Include(sp => sp.Service)
                        );
                        if (servicePayment != null)
                        {
                            _logger.LogInformation("Found service payment for ID: {PaymentId}", payment.Id);
                            var service = servicePayment.Service;
                            service.Status = ServiceStatus.Done;
                            _unitOfWork.Services.Update(service, service);
                            _logger.LogInformation("Updated Service with ID: {ServiceId}", service.Id);
                        }
                        else
                        {
                            _logger.LogWarning("ServicePayment not found for Payment ID: {PaymentId}", payment.Id);
                        }
                    }
                }

                _unitOfWork.Payments.Update(payment, payment);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Saved changes to database for payment ID: {PaymentId}", payment.Id);

                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("Transaction committed for payment ID: {PaymentId}", payment.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during webhook processing for transaction ID: {TransactionId}", ex.Message);
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest($"Webhook Error: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
                _logger.LogInformation("UnitOfWork disposed after webhook processing");
            }
        }

        private bool ValidateHmac(dynamic data, string receivedHmac)
        {
            var obj = data?.obj;
            if (obj == null) return false;

            var fields = new List<string>
            {
                obj.amount_cents?.ToString() ?? "",
                obj.order?.created_at?.ToString() ?? "",
                obj.currency?.ToString() ?? "",
                obj.error_occured?.ToString()?.ToLower() ?? "false",
                obj.has_parent_transaction?.ToString()?.ToLower() ?? "false",
                obj.id?.ToString() ?? "",
                obj.integration_id?.ToString() ?? "",
                obj.is_3d_secure?.ToString()?.ToLower() ?? "false",
                obj.is_auth?.ToString()?.ToLower() ?? "false",
                obj.is_capture?.ToString()?.ToLower() ?? "false",
                obj.is_refunded?.ToString()?.ToLower() ?? "false",
                obj.is_standalone_payment?.ToString()?.ToLower() ?? "false",
                obj.is_voided?.ToString()?.ToLower() ?? "false",
                obj.order?.id?.ToString() ?? "",
                obj.profile_id?.ToString() ?? "",
                obj.pending?.ToString()?.ToLower() ?? "false",
                obj.source_data?.pan?.ToString() ?? "",
                obj.source_data?.sub_type?.ToString() ?? "",
                obj.source_data?.type?.ToString() ?? "",
                obj.success?.ToString()?.ToLower() ?? "false"
            };

            var concatenated = string.Join("", fields);
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
            var computedHmac = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated))).Replace("-", "").ToLower();
            return computedHmac == receivedHmac;
        }
    }
}