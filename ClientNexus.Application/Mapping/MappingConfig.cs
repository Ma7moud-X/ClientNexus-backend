using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Entities.Services;

namespace ClientNexus.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Slot, SlotCreateDTO>().ReverseMap();
            CreateMap<Slot, SlotDTO>().ReverseMap();
        }
    }
}
