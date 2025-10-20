using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartAgroPlan.BLL.DTO.Irrigation.Recommendations;
using SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetBatchRecommendations;

public class GetBatchIrrigationRecommendationsHandler : IRequestHandler<GetBatchIrrigationRecommendationsCommand,
    Result<List<IrrigationRecommendationDto>>>
{
    private readonly IMediator _mediator;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GetBatchIrrigationRecommendationsHandler(
        IMediator mediator,
        IServiceScopeFactory serviceScopeFactory)
    {
        _mediator = mediator;
        _serviceScopeFactory = serviceScopeFactory;
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
        return failedResults.Count != 0
            ? Result.Fail<List<IrrigationRecommendationDto>>(failedResults)
            : Result.Ok(successfulResults);
    }
}
