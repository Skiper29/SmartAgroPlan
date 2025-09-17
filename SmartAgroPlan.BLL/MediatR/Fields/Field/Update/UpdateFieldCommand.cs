using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.Update;

public record UpdateFieldCommand(FieldUpdateDto UpdatedField) : IRequest<Result<FieldDto>>;
