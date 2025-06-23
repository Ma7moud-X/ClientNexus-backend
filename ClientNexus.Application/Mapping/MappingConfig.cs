using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Domain.Entities.Others;
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

            CreateMap<AvailableDay, AvailableDayCreateDTO>().ReverseMap();
            CreateMap<AvailableDay, AvailableDayUpdateDTO>().ReverseMap();
            CreateMap<AvailableDay, AvailableDayDTO>().ReverseMap();

            CreateMap<Appointment, AppointmentCreateDTO>().ReverseMap();
            CreateMap<Appointment, AppointmentDTO>().ReverseMap();
            CreateMap<Appointment, AppointmentDTO2>()
            .ForMember(dest => dest.ServiceProviderId,
                       opt => opt.MapFrom(src => src.ServiceProviderId))
            .ForMember(dest => dest.ServiceProviderFirstName, 
                       opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.FirstName : null))
            .ForMember(dest => dest.ServiceProviderLastName, 
                       opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.LastName : null))
            .ForMember(dest => dest.ServiceProviderMainImage,
                       opt => opt.MapFrom(src => src.ServiceProvider!.MainImage))
            .ForMember(dest => dest.ServiceProviderMainSpecialization,
                       opt => opt.MapFrom(src => src.ServiceProvider!.MainSpecialization!.Name))
            .ForMember(dest => dest.ServiceProviderCity,
                       opt => opt.MapFrom(src => src.ServiceProvider!.Addresses!.FirstOrDefault()!.City!.Name))
            .ForMember(dest => dest.SlotDate,
                       opt => opt.MapFrom(src => src.Slot.Date))
            .ForMember(dest => dest.SlotType,
                       opt => opt.MapFrom(src => src.Slot.SlotType));

            CreateMap<Appointment, AppointmentDTO3>()
            .ForMember(dest => dest.ClientFirstName,
                       opt => opt.MapFrom(src => src.Client != null ? src.Client.FirstName : null))
            .ForMember(dest => dest.ClientLastName,
                       opt => opt.MapFrom(src => src.Client != null ? src.Client.LastName : null))
            .ForMember(dest => dest.ClientMainImage,
                       opt => opt.MapFrom(src => src.Client != null ?src.Client.MainImage : null))
            .ForMember(dest => dest.SlotDate,
                       opt => opt.MapFrom(src => src.Slot.Date))
            .ForMember(dest => dest.SlotType,
                       opt => opt.MapFrom(src => src.Slot.SlotType));

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
