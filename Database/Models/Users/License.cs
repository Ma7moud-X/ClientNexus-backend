using Database.Models.Users;

namespace Database.Models
{
    public class License
    {
        public int ServiceProviderId { get; set; }
        public int Id { get; set; }

        public string LicenceNumber { get; set; } = default!;
        public string IssuingAuthority { get; set; } = default!;
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string ImageUrl { get; set; } = default!;

        public ServiceProvider? ServiceProvider { get; set; }
    }
}
