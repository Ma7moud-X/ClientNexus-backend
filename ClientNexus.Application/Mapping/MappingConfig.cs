using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Entities.Services;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Slot, SlotCreateDTO>().ReverseMap();
            CreateMap<Slot, SlotDTO>().ReverseMap();
            CreateMap<Appointment, AppointmentCreateDTO>().ReverseMap();
            CreateMap<Appointment, AppointmentDTO>().ReverseMap();
        }
    }
}
