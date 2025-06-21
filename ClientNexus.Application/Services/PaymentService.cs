using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Application.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymobPaymentService _paymobService;

        public PaymentService(IUnitOfWork unitOfWork, PaymobPaymentService paymobService)
        {
            _unitOfWork = unitOfWork;
            _paymobService = paymobService;
        }

        public async Task<PaymentResponseDTO> StartSubscriptionPayment(StartSubscriptionPaymentRequestDTO request)
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

            // Validate ServiceProviderId
            var serviceProvider = await _unitOfWork.ServiceProviders.FirstOrDefaultAsync(sp => sp.Id == request.ServiceProviderId);
            if (serviceProvider == null)
            {
                throw new ArgumentException("Service provider not found.");
            }

            var amount = _paymobService.GetSubscriptionAmount(request.SubscriptionType, request.SubscriptionTier);
            var reference = Guid.NewGuid().ToString();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (clientSecret, intentionId) = await _paymobService.StartPayment(
                    amount,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.Phone,
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
                    ServiceProviderId = request.ServiceProviderId,
                    SubscriptionTier = request.SubscriptionTier
                };

                await _unitOfWork.Payments.AddAsync(subscriptionPayment);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new PaymentResponseDTO
                {
                    ClientSecret = clientSecret,
                    IntentionId = intentionId,
                    PublicKey = _paymobService.GetPublicKey(),
                    Status = "pending"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to start subscription payment: {ex.Message}", ex);
            }
        }

        public async Task<PaymentResponseDTO> StartServicePayment(StartServicePaymentRequestDTO request)
        {
            // Validate ClientId
            var client = await _unitOfWork.Clients.FirstOrDefaultAsync(c => c.Id == request.ClientId);
            if (client == null)
            {
                throw new ArgumentException("Client not found.");
            }

            // Validate ServiceProviderId
            var serviceProvider = await _unitOfWork.ServiceProviders.FirstOrDefaultAsync(sp => sp.Id == request.ServiceProviderId);
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
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.Phone,
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
                    ClientId = request.ClientId,
                    ServiceProviderId = request.ServiceProviderId
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

                await _unitOfWork.Payments.AddAsync(servicePayment);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new PaymentResponseDTO
                {
                    ClientSecret = clientSecret,
                    IntentionId = intentionId,
                    PublicKey = _paymobService.GetPublicKey(),
                    Status = "pending"
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
      
    }
}
