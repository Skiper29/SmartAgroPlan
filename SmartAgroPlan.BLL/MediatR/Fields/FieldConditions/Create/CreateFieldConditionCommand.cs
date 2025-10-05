using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Create;

public record CreateFieldConditionCommand(FieldConditionCreateDto NewFieldCondition)
    : IRequest<Result<FieldConditionDto>>;
