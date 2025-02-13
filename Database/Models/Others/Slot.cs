using System;
using System.Collections.Generic;
using Database.Models.Users;

namespace Database.Models.Services
{
    public class Slot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        
        public Appointment? Appointment { get; set; }
        public ICollection<SlotType> SlotTypes { get; set; }
        public ICollection<SlotServiceProvider> SlotServiceProviders { get; set; }

    }
}