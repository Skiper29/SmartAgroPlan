using AutoMapper;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Mapping.Fields;

public class FieldConditionProfile : Profile
{
    public FieldConditionProfile()
    {
        CreateMap<FieldCondition, FieldConditionDto>()
            .ReverseMap();

        CreateMap<FieldCondition, FieldConditionCreateDto>()
            .ReverseMap();
    }
}
