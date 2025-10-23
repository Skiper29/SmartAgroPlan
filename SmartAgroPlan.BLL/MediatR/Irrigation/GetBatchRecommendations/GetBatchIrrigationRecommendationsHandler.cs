using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Irrigation.Recommendations;
using SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetBatchRecommendations;

public class GetBatchIrrigationRecommendationsHandler : IRequestHandler<GetBatchIrrigationRecommendationsCommand,
    Result<List<IrrigationRecommendationDto>>>
{
    private readonly ILogger<GetBatchIrrigationRecommendationsHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GetBatchIrrigationRecommendationsHandler(
        IServiceScopeFactory serviceScopeFactory, ILogger<GetBatchIrrigationRecommendationsHandler> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task<Result<List<IrrigationRecommendationDto>>> Handle(
        GetBatchIrrigationRecommendationsCommand request, CancellationToken cancellationToken)
    {
        var tasks = request.FieldIds.Select(async fieldId =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await mediator.Send(new GetIrrigationRecommendationCommand
            {
                FieldId = fieldId,
                Date = request.Date
            }, cancellationToken);
        });

        var results = await Task.WhenAll(tasks);

        var successfulResults = results.Where(r => r.IsSuccess).Select(r => r.Value).ToList();
        var failedResults = results.Where(r => r.IsFailed).SelectMany(r => r.Errors).ToList();
        if (failedResults.Count != 0)
        {
            _logger.LogWarning(
                "Під час пакетного розрахунку рекомендацій {FailedCount} з {TotalCount} полів не вдалося обробити.",
                failedResults.Count, request.FieldIds.Count);

            foreach (var error in failedResults)
                _logger.LogWarning("Помилка пакетної обробки: {ErrorMessage}", error.Message);
        }

        return Result.Ok(successfulResults);
    }
}
