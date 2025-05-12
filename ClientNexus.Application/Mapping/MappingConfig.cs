using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;

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
            CreateMap<Question, QuestionResponsePDTO>()
                .ForMember(dest => dest.ClientBirthDate, opt => opt.MapFrom(src => src.Client!.BirthDate))
                .ForMember(dest => dest.ClientGender, opt => opt.MapFrom(src => src.Client!.Gender))
                .ForMember(dest => dest.ServiceProviderFirstName, opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.FirstName : null))
                .ForMember(dest => dest.ServiceProviderLastName, opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.LastName : null));
            CreateMap<Question, QuestionResponseCDTO>()
                .ForMember(dest => dest.ServiceProviderFirstName, opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.FirstName : null))
                .ForMember(dest => dest.ServiceProviderLastName, opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.LastName : null));
        }
    }
}
