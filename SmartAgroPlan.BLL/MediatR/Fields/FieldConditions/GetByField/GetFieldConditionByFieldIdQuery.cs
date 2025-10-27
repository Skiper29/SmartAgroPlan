using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetByField;

public record GetFieldConditionByFieldIdQuery(int FieldId) : IRequest<Result<List<FieldConditionDto>>>;
