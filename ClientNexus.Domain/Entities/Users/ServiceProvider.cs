using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using NetTopologySuite.Geometries;

namespace ClientNexus.Domain.Entities.Users
{
    public class ServiceProvider : BaseUser
    {
        public string Description { get; set; } = default!;
        public Point? CurrentLocation { get; set; }
        public DateTime? LastLocationUpdateTime { get; set; }
        public float Rate { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailableForEmergency { get; set; }
        public int YearsOfExperience { get; set; }
        public string ImageIDUrl { get; set; } = default!;
        public string ImageNationalIDUrl { get; set; } = default!;
        public int Office_consultation_price { get; set; }
        public int Telephone_consultation_price { get; set; }
        public int main_specializationID {  get; set; }
        public Specialization? MainSpecialization { get; set; }
        public SubscriptionType SubType { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }

        public int TypeId { get; set; }
        public ServiceProviderType? Type { get; set; }

        public ICollection<ServiceProviderSpecialization>? ServiceProviderSpecializations { get; set; }
        public ICollection<Specialization>? Specializations { get; set; }

        public ICollection<OfficeImageUrl>? OfficeImageUrls { get; set; }

        public int? ApprovedById { get; set; }
        public Admin? ApprovingAdmin { get; set; }

        public ICollection<Problem>? Problems { get; set; }
        public ICollection<Service>? ServicesProvided { get; set; }
        public ICollection<SubscriptionPayment>? SubscriptionPayments { get; set; }

        public ICollection<Slot>? Slots { get; set; }

        public ICollection<ClientServiceProviderFeedback>? ClientServiceProviderFeedbacks { get; set; }
        public ICollection<Client>? ClientsWithFeedbacks { get; set; }

        public ICollection<AppointmentCost>? AppointmentCosts { get; set; }
    }
}
