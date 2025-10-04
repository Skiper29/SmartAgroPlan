using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetAll;

public record GetAllFieldConditionsQuery : IRequest<Result<IEnumerable<FieldConditionDto>>>;
