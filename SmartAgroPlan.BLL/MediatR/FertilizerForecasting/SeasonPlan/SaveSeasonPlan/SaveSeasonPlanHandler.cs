using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.SeasonPlan.SaveSeasonPlan;

public class SaveSeasonPlanHandler : IRequestHandler<SaveSeasonPlanCommand, Result<int>>
{
    private readonly IFertilizerCalculationService _calculationService;
    private readonly ILogger<SaveSeasonPlanHandler> _logger;
    private readonly IFertilizerPlanManagementService _managementService;

    public SaveSeasonPlanHandler(
        IFertilizerCalculationService calculationService,
        IFertilizerPlanManagementService managementService,
        ILogger<SaveSeasonPlanHandler> logger)
    {
        _calculationService = calculationService;
        _managementService = managementService;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(SaveSeasonPlanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Збереження плану внесення добрив на сезон для поля {FieldId}", request.FieldId);

            // First calculate the plan
            var plan = await _calculationService.CalculateSeasonPlanAsync(
                request.FieldId,
                request.TargetYield,
                request.CustomSowingDate);

            // Then save it to database
            var savedId = await _managementService.SaveSeasonPlanAsync(plan, request.FieldId);

            _logger.LogInformation(
                "Успішно збережено план внесення добрив на сезон для поля {FieldId} з ідентифікатором {PlanId}",
                request.FieldId,
                savedId);

            return Result.Ok(savedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при збереженні плану внесення добрив на сезон для поля {FieldId}",
                request.FieldId);
            return Result.Fail(new Error($"Помилка при збереженні плану внесення добрив: {ex.Message}"));
        }
    }
}
