using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.GetAll;

public record GetAllFieldsQuery : IRequest<Result<IEnumerable<FieldDto>>>;
