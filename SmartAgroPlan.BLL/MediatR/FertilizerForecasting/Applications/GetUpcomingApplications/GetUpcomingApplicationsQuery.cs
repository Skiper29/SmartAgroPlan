using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetUpcomingApplications;

public record GetUpcomingApplicationsQuery(
    int FieldId,
    int DaysAhead = 14
) : IRequest<Result<List<FertilizerApplicationDto>>>;
