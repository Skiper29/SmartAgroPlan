using FluentResults;
using MediatR;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Delete;

public record DeleteFieldConditionCommand(int Id) : IRequest<Result<Unit>>;
