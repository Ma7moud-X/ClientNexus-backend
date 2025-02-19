using Database.Models.Users;
using Database.Models.Others;

namespace Database.Models
{
    public class Address
    {
        public int BaseUserId { get; set; }
        public int Id { get; set; }

        public string DetailedAddress { get; set; } = default!;
        public string? Neighborhood { get; set; }
        public string? MapUrl { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }

        public BaseUser? BaseUser { get; set; }
    }
}
