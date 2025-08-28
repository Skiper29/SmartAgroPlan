using AutoMapper;
using SmartAgroPlan.BLL.DTO.Fields.FieldCropHistory;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Mapping.Fields;

public class FieldCropHistoryProfile : Profile
{
    public FieldCropHistoryProfile()
    {
        CreateMap<FieldCropHistory, FieldCropHistoryDto>().ReverseMap();
        CreateMap<FieldCropHistory, FieldCropHistoryCreateDto>().ReverseMap();
        CreateMap<FieldCropHistory, FieldCropHistoryCreateUpdateDto>().ReverseMap();
    }
}
