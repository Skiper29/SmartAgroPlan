using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Soil;

namespace SmartAgroPlan.BLL.MediatR.Fields.Soil.GetAll;

public record GetAllSoilsQuery : IRequest<Result<IEnumerable<SoilDto>>>;
