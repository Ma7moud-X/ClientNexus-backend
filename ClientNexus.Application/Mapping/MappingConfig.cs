using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Services;

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

            CreateMap<Question, QuestionCreateDTO>().ReverseMap();
            CreateMap<Question, QuestionResponseDTO>().ReverseMap();
        }
    }
}
