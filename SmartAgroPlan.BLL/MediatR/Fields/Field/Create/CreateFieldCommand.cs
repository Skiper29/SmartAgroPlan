using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.Create;

public record CreateFieldCommand(FieldCreateDto NewField) : IRequest<Result<FieldDto>>;
