using AutoMapper;
using SmartAgroPlan.BLL.DTO.Fields.FieldCondition;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Mapping.Fields;

public class FieldConditionProfile : Profile
{
    public FieldConditionProfile()
    {
        CreateMap<FieldCondition, FieldConditionDto>()
            .ReverseMap();

        CreateMap<FieldCondition, FieldConditionCreateUpdateDto>()
            .ReverseMap();

        CreateMap<FieldCondition, FieldConditionCreateDto>()
            .IncludeBase<FieldCondition, FieldConditionCreateUpdateDto>()
            .ReverseMap()
            .IncludeBase<FieldConditionCreateUpdateDto, FieldCondition>();

        CreateMap<FieldCondition, FieldConditionUpdateDto>()
            .IncludeBase<FieldCondition, FieldConditionCreateUpdateDto>()
            .ReverseMap()
            .IncludeBase<FieldConditionCreateUpdateDto, FieldCondition>();
    }
}
