using System;
using Database.Models.Users;

namespace Database.Models.Services
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign key
        public int ClientId { get; set; }
        // Navigation property
        public Client Client { get; set; }
    }
}