using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetApplicationSummary;

public record GetApplicationSummaryQuery(
    int FieldId,
    DateTime FromDate,
    DateTime ToDate
) : IRequest<Result<NutrientApplicationSummaryDto>>;
