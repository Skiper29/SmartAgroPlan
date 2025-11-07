using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.RecordApplication;

public class RecordApplicationHandler : IRequestHandler<RecordApplicationCommand, Result<int>>
{
    private readonly ILogger<RecordApplicationHandler> _logger;
    private readonly IFertilizerApplicationRecordService _recordService;

    public RecordApplicationHandler(
        IFertilizerApplicationRecordService recordService,
        ILogger<RecordApplicationHandler> logger)
    {
        _recordService = recordService;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(RecordApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Запис внесення добрив для поля {FieldId} на дату {ApplicationDate}",
                request.FieldId,
                request.ApplicationDate);

            var recordId = await _recordService.RecordApplicationAsync(
                request.FieldId,
                request.ApplicationDate,
                request.ApplicationMethodId,
                request.ProductsUsed,
                request.ApplicationPlanId,
                request.Notes,
                request.Temperature,
                request.WindSpeed,
                request.Humidity);

            _logger.LogInformation(
                "Успішно записано внесення добрив з ID {RecordId} для поля {FieldId}",
                recordId,
                request.FieldId);

            return Result.Ok(recordId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при записі внесення добрив для поля {FieldId} на дату {ApplicationDate}",
                request.FieldId,
                request.ApplicationDate);
            return Result.Fail(new Error($"Помилка при записі внесення добрив: {ex.Message}"));
        }
    }
}