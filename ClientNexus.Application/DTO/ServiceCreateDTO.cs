using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTO
{
    public class ServiceCreateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
        [Required]
        //public ServiceType ServiceType { get; set; }
        public decimal Price { get; set; }
        [Required]
        public int ClientId { get; set; }
        public int? ServiceProviderId { get; set; }

    }
}
