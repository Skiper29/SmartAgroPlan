using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.SeasonPlan.CalculateSeasonPlan;

public class CalculateSeasonPlanHandler : IRequestHandler<CalculateSeasonPlanQuery, Result<SeasonFertilizerPlanDto>>
{
    private readonly IFertilizerCalculationService _calculationService;
    private readonly ILogger<CalculateSeasonPlanHandler> _logger;
    private readonly IMapper _mapper;

    public CalculateSeasonPlanHandler(
        IFertilizerCalculationService calculationService,
        IMapper mapper,
        ILogger<CalculateSeasonPlanHandler> logger)
    {
        _calculationService = calculationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<SeasonFertilizerPlanDto>> Handle(
        CalculateSeasonPlanQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Розрахунок плану внесення добрив на сезон для поля {FieldId}", request.FieldId);

            var plan = await _calculationService.CalculateSeasonPlanAsync(
                request.FieldId,
                request.TargetYield,
                request.CustomSowingDate);

            var planDto = _mapper.Map<SeasonFertilizerPlanDto>(plan);

            _logger.LogInformation(
                "Успішно розраховано план внесення добрив на сезон для поля {FieldId} з {ApplicationCount} внесеннями",
                request.FieldId,
                planDto.Applications.Count);

            return Result.Ok(planDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Поле з ідентифікатором {FieldId} не знайдено", request.FieldId);
            return Result.Fail(new Error(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Неправильна операція при розрахунку плану для поля {FieldId}", request.FieldId);
            return Result.Fail(new Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при розрахунку плану внесення добрив на сезон для поля {FieldId}",
                request.FieldId);
            return Result.Fail(new Error("Помилка при розрахунку плану внесення добрив на сезон."));
        }
    }
}
