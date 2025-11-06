using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetApplicationsForDateRange;

public record GetApplicationsForDateRangeQuery(
    int FieldId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<List<FertilizerApplicationDto>>>;
