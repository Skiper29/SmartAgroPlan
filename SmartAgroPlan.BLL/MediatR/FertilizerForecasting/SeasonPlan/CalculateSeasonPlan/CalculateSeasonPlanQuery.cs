using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.SeasonPlan.CalculateSeasonPlan;

public record CalculateSeasonPlanQuery(
    int FieldId,
    double TargetYield,
    DateTime? CustomSowingDate = null
) : IRequest<Result<SeasonFertilizerPlanDto>>;
