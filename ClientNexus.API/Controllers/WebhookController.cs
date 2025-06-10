using Azure.Core;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.API.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly string _hmacSecret;
        private readonly IUnitOfWork _unitOfWork;

        public WebhookController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _hmacSecret = configuration["Paymob:HmacSecret"];
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(body);

                var receivedHmac = Request.Headers["hmac"].ToString();
                if (string.IsNullOrEmpty(receivedHmac) || !ValidateHmac(data, receivedHmac))
                {
                    return Unauthorized("Invalid HMAC signature");
                }

                string type = data.type?.ToString();
                var obj = data.obj;
                string transactionId = obj?.id?.ToString();
                string success = obj?.success?.ToString()?.ToLower();

                if (type != "TRANSACTION" || string.IsNullOrEmpty(transactionId))
                {
                    return BadRequest("Invalid webhook payload");
                }

                // Assuming IntentionId in the database is the transaction ID
                var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.IntentionId == transactionId);
                if (payment == null)
                {
                    return NotFound("Payment not found");
                }

                await _unitOfWork.BeginTransactionAsync();

                payment.WebhookStatus = success == "true" ? "success" : "failed";
                payment.Status = success == "true" ? PaymentStatus.Completed : PaymentStatus.Failed;

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
                                _ => serviceProvider.SubType // Keep existing if invalid
                            };
                            _unitOfWork.ServiceProviders.Update(serviceProvider, serviceProvider);
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
                            var service = servicePayment.Service;
                            service.Status = ServiceStatus.Done;
                            _unitOfWork.Services.Update(service, service);
                        }
                    }
                }

                _unitOfWork.Payments.Update(payment, payment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest($"Webhook Error: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        private bool ValidateHmac(dynamic data, string receivedHmac)
        {
            var obj = data?.obj;
            if (obj == null) return false;

            var fields = new List<string>
            {
                obj.amount_cents?.ToString() ?? "",
                obj.order?.created_at?.ToString() ?? "", // Fixed: Access created_at from order
                obj.currency?.ToString() ?? "", // Currency might be missing; default to empty
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
                obj.order?.id?.ToString() ?? "", // Fixed: Access order_id from order
                obj.profile_id?.ToString() ?? "", // Use profile_id as owner (merchant ID)
                obj.pending?.ToString()?.ToLower() ?? "false",
                obj.source_data?.pan?.ToString() ?? "", // Fixed: Access nested source_data
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