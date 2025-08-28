using AutoMapper;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Mapping.Fields;

public class SoilProfile : Profile
{
    public SoilProfile()
    {
        CreateMap<Soil, SoilDto>().ReverseMap();
        CreateMap<Soil, SoilCreateDto>().ReverseMap();
        CreateMap<Soil, SoilUpdateDto>().ReverseMap();
        CreateMap<Soil, SoilCreateUpdateDto>().ReverseMap();
    }
}
