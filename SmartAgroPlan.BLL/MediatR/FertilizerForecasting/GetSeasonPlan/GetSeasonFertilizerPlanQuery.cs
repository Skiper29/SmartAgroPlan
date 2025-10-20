using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.GetSeasonPlan;

public record GetSeasonFertilizerPlanQuery(int FieldId, double? TargetYieldOverride)
    : IRequest<Result<SeasonFertilizerPlanDto>>;
