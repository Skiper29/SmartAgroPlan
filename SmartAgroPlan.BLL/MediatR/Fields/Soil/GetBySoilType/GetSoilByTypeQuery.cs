using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Fields.Soil;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.MediatR.Fields.Soil.GetBySoilType;

public record GetSoilByTypeQuery(SoilType SoilType)
    : IRequest<Result<SoilDto>>;