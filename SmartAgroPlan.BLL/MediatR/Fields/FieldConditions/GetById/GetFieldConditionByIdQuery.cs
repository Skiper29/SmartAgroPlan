using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetById;

public record GetFieldConditionByIdQuery(int Id) : IRequest<Result<FieldConditionDto>>;
