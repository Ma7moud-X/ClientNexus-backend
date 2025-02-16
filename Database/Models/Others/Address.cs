using Database.Models.Users;

namespace Database.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string DetailedAddress { get; set; } = default!;
        public string Neighborhood { get; set; } = default!;
        public string City { get; set; } = default!;
        public string MapUrl { get; set; } = default!;

        public int ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider { get; set; }
    }
}
