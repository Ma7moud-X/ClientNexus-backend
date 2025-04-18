﻿using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTO
{
    public class ServiceDTO
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
        public decimal Price { get; set; }
        [Required]
        public int ClientId { get; set; }
        public int? ServiceProviderId { get; set; }

    }
}
