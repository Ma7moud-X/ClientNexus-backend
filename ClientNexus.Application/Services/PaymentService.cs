using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.Enums;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System; // NEW: Added for DateTime calculations
using Microsoft.EntityFrameworkCore; // NEW: Added for Include in queries


namespace ClientNexus.Application.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymobPaymentService _paymobService;
        private readonly HttpClient _httpClient;
        private readonly string _paymobAPIkey;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IUnitOfWork unitOfWork, PaymobPaymentService paymobService, IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _paymobService = paymobService;
            _httpClient = httpClientFactory.CreateClient();
            _paymobAPIkey = configuration["Paymob:APIKey"];
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaymentResponseDTO> StartSubscriptionPayment(StartSubscriptionPaymentRequestDTO request, int serviceProviderId)
        {
            // Validate SubscriptionType
            if (!new[] { "Monthly", "Quarterly", "Yearly" }.Contains(request.SubscriptionType))
            {
                throw new ArgumentException("Invalid subscription type. Must be 'Monthly', 'Quarterly', or 'Yearly'.");
            }

            // Validate SubscriptionTier
            if (!new[] { "Normal", "Advanced" }.Contains(request.SubscriptionTier))
            {
                throw new ArgumentException("Invalid subscription tier. Must be 'Normal' or 'Advanced'.");
            }

            // Validate ServiceProviderId and retrieve user data
            var serviceProvider = await _unitOfWork.ServiceProviders.FirstOrDefaultAsync(sp => sp.Id == serviceProviderId);
            if (serviceProvider == null)
            {
                throw new ArgumentException("Service provider not found.");
            }
            var userData = await _unitOfWork.BaseUsers.FirstOrDefaultAsync(u => u.Id == serviceProviderId);
            if (userData == null || string.IsNullOrEmpty(userData.Email) || string.IsNullOrEmpty(userData.FirstName) ||
                string.IsNullOrEmpty(userData.LastName) || string.IsNullOrEmpty(userData.PhoneNumber))
            {
                throw new ArgumentException("User data not found or incomplete for the service provider.");
            }

            var amount = _paymobService.GetSubscriptionAmount(request.SubscriptionType, request.SubscriptionTier);
            var reference = Guid.NewGuid().ToString();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (clientSecret, intentionId) = await _paymobService.StartPayment(
                    amount,
                    userData.Email,
                    userData.FirstName,
                    userData.LastName,
                    userData.PhoneNumber,
                    reference,
                    new[] { $"Subscription ({request.SubscriptionType}, {request.SubscriptionTier})" });

                // Create a SubscriptionPayment directly
                var subscriptionPayment = new SubscriptionPayment
                {
                    Signature = reference,
                    Amount = amount,
                    ReferenceNumber = reference,
                    PaymentGateway = "Paymob",
                    Status = PaymentStatus.Pending,
                    PaymentType = PaymentType.Subscription,
                    IntentionId = intentionId,
                    ClientSecret = clientSecret,
                    WebhookStatus = "pending",
                    SubscriptionType = request.SubscriptionType switch
                    {
                        "Monthly" => 'M',
                        "Quarterly" => 'Q',
                        "Yearly" => 'Y',
                        _ => throw new ArgumentException("Invalid subscription type")
                    },
                    ServiceProviderId = serviceProviderId,
                    SubscriptionTier = request.SubscriptionTier
                };

                await _unitOfWork.SubscriptionPayments.AddAsync(subscriptionPayment);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new PaymentResponseDTO
                {
                    ClientSecret = clientSecret,
                    IntentionId = intentionId,
                    PublicKey = _paymobService.GetPublicKey(),
                    Status = "pending",
                    ReferenceNumber = reference
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to start subscription payment: {ex.Message}", ex);
            }
        }

        public async Task<PaymentResponseDTO> StartServicePayment(StartServicePaymentRequestDTO request, int clientId)
        {
            // Validate ClientId and retrieve user data
            var client = await _unitOfWork.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null)
            {
                throw new ArgumentException("Client not found.");
            }
            var userData = await _unitOfWork.BaseUsers.FirstOrDefaultAsync(u => u.Id == clientId);
            if (userData == null || string.IsNullOrEmpty(userData.Email) || string.IsNullOrEmpty(userData.FirstName) ||
                string.IsNullOrEmpty(userData.LastName) || string.IsNullOrEmpty(userData.PhoneNumber))
            {
                throw new ArgumentException("User data not found or incomplete for the client.");
            }

            // Validate ServiceProviderId from request
            var serviceProviderId = request.ServiceProviderId;
            var serviceProvider = await _unitOfWork.ServiceProviders.FirstOrDefaultAsync(sp => sp.Id == serviceProviderId);
            if (serviceProvider == null)
            {
                throw new ArgumentException("Service provider not found.");
            }

            var reference = Guid.NewGuid().ToString();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (clientSecret, intentionId) = await _paymobService.StartPayment(
                    request.Amount,
                    userData.Email,
                    userData.FirstName,
                    userData.LastName,
                    userData.PhoneNumber,
                    reference,
                    new[] { request.ServiceName });

                // Determine ServiceType dynamically based on ServiceName
                var serviceType = DetermineServiceType(request.ServiceName);

                var service = new Service
                {
                    Name = request.ServiceName,
                    Description = request.ServiceName,
                    Status = ServiceStatus.Pending,
                    ServiceType = serviceType,
                    Price = request.Amount,
                    ClientId = clientId,
                    ServiceProviderId = serviceProviderId
                };

                await _unitOfWork.Services.AddAsync(service);
                await _unitOfWork.SaveChangesAsync();

                // Create a ServicePayment directly
                var servicePayment = new ServicePayment
                {
                    Signature = reference,
                    Amount = request.Amount,
                    ReferenceNumber = reference,
                    PaymentGateway = "Paymob",
                    Status = PaymentStatus.Pending,
                    PaymentType = PaymentType.Service,
                    IntentionId = intentionId,
                    ClientSecret = clientSecret,
                    WebhookStatus = "pending",
                    ServiceId = service.Id,
                    ServiceName = request.ServiceName
                };

                await _unitOfWork.ServicePayments.AddAsync(servicePayment);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new PaymentResponseDTO
                {
                    ClientSecret = clientSecret,
                    IntentionId = intentionId,
                    PublicKey = _paymobService.GetPublicKey(),
                    Status = "pending",
                    ReferenceNumber = reference
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to start service payment: {ex.Message}", ex);
            }
        }

        public async Task<VerifyPaymentResponseDTO> VerifyPayment(string intentionId)
        {
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.IntentionId == intentionId);

            if (payment == null || string.IsNullOrEmpty(payment.WebhookStatus))
            {
                return new VerifyPaymentResponseDTO { Status = "pending" };
            }

            return new VerifyPaymentResponseDTO { Status = payment.WebhookStatus };
        }


        private ServiceType DetermineServiceType(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                return ServiceType.Appointment; // Default
            }

            serviceName = serviceName.ToLower();
            if (serviceName.Contains("emergency"))
            {
                return ServiceType.Emergency;
            }
            else if (serviceName.Contains("consultation"))
            {
                return ServiceType.Consultation;
            }
            else if (serviceName.Contains("question"))
            {
                return ServiceType.Question;
            }
            else
            {
                return ServiceType.Appointment; // Default
            }
        }
<<<<<<< HEAD


=======
>>>>>>> 2316c94f471c4148ec77dd14a2d72f594f73136a
        public async Task<GetPaymentStatusResponseDTO> GetPaymentStatus(string referenceNumber)
        {
            if (string.IsNullOrEmpty(referenceNumber))
            {
                throw new ArgumentException("Reference number is required.");
            }

            // Step 1: Authenticate to get Bearer Token
            var authRequest = new { api_key = _paymobAPIkey };
            var authContent = new StringContent(JsonConvert.SerializeObject(authRequest), Encoding.UTF8, "application/json");
            authContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var authResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", authContent);
            authResponse.EnsureSuccessStatusCode();
            var authData = JsonConvert.DeserializeObject<dynamic>(await authResponse.Content.ReadAsStringAsync());
            var token = authData.token?.ToString();
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Invalid or empty token received from authentication.");
            }

            // Step 2: Retrieve transaction status
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var inquiryRequest = new { merchant_order_id = referenceNumber }; // Correct field name
            var inquiryContent = new StringContent(JsonConvert.SerializeObject(inquiryRequest), Encoding.UTF8, "application/json");
            inquiryContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var inquiryResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders/transaction_inquiry", inquiryContent);

            if (!inquiryResponse.IsSuccessStatusCode)
            {
                var errorContent = await inquiryResponse.Content.ReadAsStringAsync();
                var requestBody = await inquiryContent.ReadAsStringAsync();
                throw new HttpRequestException($"Transaction inquiry failed: {inquiryResponse.StatusCode} - {errorContent} (Request: {requestBody})");
            }

            var inquiryData = JsonConvert.DeserializeObject<dynamic>(await inquiryResponse.Content.ReadAsStringAsync());
            var transactionStatus = inquiryData.data?.txn_response_code?.ToString(); // Use txn_response_code from data

            if (string.IsNullOrEmpty(transactionStatus))
            {
                // Fallback to success flag if txn_response_code is missing
                transactionStatus = inquiryData.success == true ? "APPROVED" : "FAILED";
            }

            // Map Paymob status to your enum
            var paymentStatus = transactionStatus == "APPROVED" ? PaymentStatus.Completed :
                               transactionStatus == "PENDING" ? PaymentStatus.Pending :
                               transactionStatus == "FAILED" ? PaymentStatus.Failed :
                               PaymentStatus.Failed; // Default to Failed for unknown statuses

            // Update database
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber);
            if (payment != null)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync(); // NEW: Start transaction for atomic updates

                    // Ensure the entity is tracked and marked as modified
                    _unitOfWork.Payments.Update(payment); // Explicitly mark as modified
                    payment.Status = paymentStatus;
                    payment.WebhookStatus = transactionStatus;

                    // NEW: Check if this is a subscription payment and update ServiceProvider
                    if (payment.PaymentType == PaymentType.Subscription && paymentStatus == PaymentStatus.Completed)
                    {
                        var subscriptionPayment = await _unitOfWork.SubscriptionPayments.FirstOrDefaultAsync(
                            sp => sp.ReferenceNumber == referenceNumber,
                            q => q.Include(sp => sp.ServiceProvider) // NEW: Include ServiceProvider
                        );
                        if (subscriptionPayment != null)
                        {
                            var serviceProvider = subscriptionPayment.ServiceProvider;
                            if (serviceProvider != null)
                            {
                                // NEW: Update SubscriptionStatus
                                serviceProvider.SubscriptionStatus = SubscriptionStatus.Active;

                                // NEW: Update SubscriptionExpiryDate
                                serviceProvider.SubscriptionExpiryDate = subscriptionPayment.SubscriptionType switch
                                {
                                    'M' => DateTime.UtcNow.AddMonths(1),
                                    'Q' => DateTime.UtcNow.AddMonths(3),
                                    'Y' => DateTime.UtcNow.AddYears(1),
                                    _ => DateTime.UtcNow.AddMonths(1) // Default to 1 month
                                };

                                // NEW: Update SubscriptionType
                                serviceProvider.SubType = subscriptionPayment.SubscriptionTier switch
                                {
                                    "Normal" => SubscriptionType.Basic,
                                    "Advanced" => SubscriptionType.Premium,
                                    _ => serviceProvider.SubType // Retain existing if invalid
                                };

                                // NEW: Update IsFeatured based on SubscriptionTier
                                serviceProvider.IsFeatured= subscriptionPayment.SubscriptionTier == "Advanced";

                                // NEW: Mark ServiceProvider as modified
                                _unitOfWork.ServiceProviders.Update(serviceProvider);
                            }
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync(); // NEW: Commit transaction
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // NEW: Rollback on error
                    throw new Exception($"Failed to update payment and service provider status: {ex.Message}", ex);
                }
            }

            return new GetPaymentStatusResponseDTO
            {
                ReferenceNumber = referenceNumber,
                Status = transactionStatus,
                PaymentStatus = paymentStatus.ToString()
            };
        }
<<<<<<< HEAD
=======

>>>>>>> 2316c94f471c4148ec77dd14a2d72f594f73136a
    }
}