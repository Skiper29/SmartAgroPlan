using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Soil;

namespace SmartAgroPlan.BLL.MediatR.Fields.Soil.GetById;

public record GetSoilByIdQuery(int Id) 
    : IRequest<Result<SoilDto>>;