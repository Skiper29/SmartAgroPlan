using FluentResults;
using MediatR;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.SeasonPlan.SaveSeasonPlan;

public record SaveSeasonPlanCommand(
    int FieldId,
    double TargetYield,
    DateTime? CustomSowingDate = null
) : IRequest<Result<int>>;
