using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.CurrentRecommendation;

public class
    GetCurrentRecommendationHandler : IRequestHandler<GetCurrentRecommendationQuery, Result<CurrentRecommendationDto>>
{
    private readonly IFertilizerCalculationService _calculationService;
    private readonly ILogger<GetCurrentRecommendationHandler> _logger;
    private readonly IMapper _mapper;

    public GetCurrentRecommendationHandler(
        IFertilizerCalculationService calculationService,
        IMapper mapper,
        ILogger<GetCurrentRecommendationHandler> logger)
    {
        _calculationService = calculationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CurrentRecommendationDto>> Handle(
        GetCurrentRecommendationQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Отримання поточної рекомендації щодо внесення добрив для поля {FieldId}",
                request.FieldId);

            var recommendation = await _calculationService.GetCurrentRecommendationAsync(request.FieldId);
            var recommendationDto = _mapper.Map<CurrentRecommendationDto>(recommendation);

            _logger.LogInformation(
                "Поточна рекомендація для поля {FieldId}: Пріоритет {Priority}, Застосувати зараз: {ShouldApplyNow}",
                request.FieldId,
                recommendationDto.Priority,
                recommendationDto.ShouldApplyNow);

            return Result.Ok(recommendationDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Поле з ідентифікатором {FieldId} не знайдено", request.FieldId);
            return Result.Fail(new Error(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при отриманні поточної рекомендації щодо внесення добрив для поля {FieldId}",
                request.FieldId);
            return Result.Fail(new Error("Помилка при отриманні поточної рекомендації щодо внесення добрив."));
        }
    }
}
