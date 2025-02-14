
using Database.Models.Users;

namespace Database.Models
{
     public class Address
    {
        public int Id { get; set; }
        public string DetailedAddress { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string MapUrl { get; set; }

        public int ServiceProviderId {get; set;}
        public ServiceProvider ServiceProvider { get; set; }

    }
}