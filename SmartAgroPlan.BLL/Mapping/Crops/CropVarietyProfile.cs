using AutoMapper;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.DAL.Entities.Crops;

namespace SmartAgroPlan.BLL.Mapping.Crops;

public class CropVarietyProfile : Profile
{
    public CropVarietyProfile()
    {
        CreateMap<CropVariety, CropVarietyDto>()
            .ForMember(c => c.OptimalSoil, opt => opt.MapFrom(src => src.OptimalSoil!.Type));
        CreateMap<CropVariety, CropVarietyCreateDto>().ReverseMap();
        CreateMap<CropVariety, CropVarietyUpdateDto>().ReverseMap();
        CreateMap<CropVariety, CropVarietyCreateUpdateDto>().ReverseMap();
    }
}
