using FluentResults;
using MediatR;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.Delete;

public record DeleteFieldCommand(int Id) : IRequest<Result<Unit>>;
