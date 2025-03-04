﻿using ClientNexus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTO
{
    public class SlotDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ServiceProviderId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public SlotStatus Status { get; set; }
        [Required]
        public SlotType SlotType { get; set; }
    }
}
