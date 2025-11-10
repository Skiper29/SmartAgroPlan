using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetApplicationsForDateRange;

public class
    GetApplicationsForDateRangeHandler : IRequestHandler<GetApplicationsForDateRangeQuery,
    Result<List<FertilizerApplicationDto>>>
{
    private readonly IFertilizerCalculationService _calculationService;
    private readonly ILogger<GetApplicationsForDateRangeHandler> _logger;
    private readonly IMapper _mapper;

    public GetApplicationsForDateRangeHandler(
        IFertilizerCalculationService calculationService,
        IMapper mapper,
        ILogger<GetApplicationsForDateRangeHandler> logger)
    {
        _calculationService = calculationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<FertilizerApplicationDto>>> Handle(
        GetApplicationsForDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Отримання заявок на внесення добрив для поля {FieldId} у діапазоні дат {StartDate} - {EndDate}",
                request.FieldId,
                request.StartDate,
                request.EndDate);

            var applications = await _calculationService.GetApplicationsForDateRangeAsync(
                request.FieldId,
                DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc));

            var applicationsDto = _mapper.Map<List<FertilizerApplicationDto>>(applications);

            _logger.LogInformation(
                "Отримано {ApplicationCount} заявок на внесення добрив для поля {FieldId}",
                applicationsDto.Count,
                request.FieldId);

            return Result.Ok(applicationsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Помилка при отриманні заявок на внесення добрив для поля {FieldId} у діапазоні дат {StartDate} - {EndDate}",
                request.FieldId,
                request.StartDate,
                request.EndDate);
            return Result.Fail(new Error("Помилка при отриманні заявок на внесення добрив."));
        }
    }
}
