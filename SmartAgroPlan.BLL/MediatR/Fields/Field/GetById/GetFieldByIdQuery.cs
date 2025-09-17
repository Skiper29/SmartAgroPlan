using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.GetById;

public record GetFieldByIdQuery(int Id) : IRequest<Result<FieldDto>>;
