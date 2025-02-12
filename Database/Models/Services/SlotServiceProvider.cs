using Database.Models.Users;

namespace Database.Models.Services
{
    public class SlotServiceProvider
    {
        public int SlotId { get; set; }
        public int ServiceProviderId { get; set; }

        public Slot Slot { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}