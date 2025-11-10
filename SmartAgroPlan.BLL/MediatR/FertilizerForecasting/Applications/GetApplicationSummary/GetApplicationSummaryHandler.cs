using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetApplicationSummary;

public class
    GetApplicationSummaryHandler : IRequestHandler<GetApplicationSummaryQuery, Result<NutrientApplicationSummaryDto>>
{
    private readonly ILogger<GetApplicationSummaryHandler> _logger;
    private readonly IFertilizerPlanManagementService _managementService;
    private readonly IMapper _mapper;

    public GetApplicationSummaryHandler(
        IFertilizerPlanManagementService managementService,
        IMapper mapper,
        ILogger<GetApplicationSummaryHandler> logger)
    {
        _managementService = managementService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<NutrientApplicationSummaryDto>> Handle(
        GetApplicationSummaryQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Отримання підсумку внесення добрив для поля {FieldId} у діапазоні дат {FromDate} - {ToDate}",
                request.FieldId,
                request.FromDate,
                request.ToDate);

            var summary = await _managementService.GetApplicationSummaryAsync(
                request.FieldId,
                DateTime.SpecifyKind(request.FromDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(request.ToDate, DateTimeKind.Utc));

            var summaryDto = _mapper.Map<NutrientApplicationSummaryDto>(summary);

            _logger.LogInformation(
                "Отримано підсумок внесення добрив для поля {FieldId}: {CompletedApplications} завершених заявок, {PendingApplications} очікуючих заявок",
                request.FieldId,
                summaryDto.CompletedApplications,
                summaryDto.PendingApplications);

            return Result.Ok(summaryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Помилка при отриманні підсумку внесення добрив для поля {FieldId} у діапазоні дат {FromDate} - {ToDate}",
                request.FieldId,
                request.FromDate,
                request.ToDate);
            return Result.Fail(new Error("Помилка при отриманні підсумку внесення добрив."));
        }
    }
}
