using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Entities.Services;

namespace ClientNexus.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Slot, SlotCreateDTO>().ReverseMap();
            CreateMap<Slot, SlotDTO>().ReverseMap();

            CreateMap<AvailableDay, AvailableDayCreateDTO>().ReverseMap();
            CreateMap<AvailableDay, AvailableDayUpdateDTO>().ReverseMap();
            CreateMap<AvailableDay, AvailableDayDTO>().ReverseMap();

            CreateMap<Appointment, AppointmentCreateDTO>().ReverseMap();
            CreateMap<Appointment, AppointmentDTO>().ReverseMap();

            CreateMap<Question, QuestionCreateDTO>().ReverseMap();
            CreateMap<Question, QuestionResponseDTO>().ReverseMap();
        }
    }
}
