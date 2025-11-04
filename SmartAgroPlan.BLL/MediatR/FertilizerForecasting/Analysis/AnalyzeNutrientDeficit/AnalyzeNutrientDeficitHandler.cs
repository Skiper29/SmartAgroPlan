using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Analysis.AnalyzeNutrientDeficit;

public class
    AnalyzeNutrientDeficitHandler : IRequestHandler<AnalyzeNutrientDeficitQuery, Result<NutrientDeficitAnalysisDto>>
{
    private readonly IFertilizerCalculationService _calculationService;
    private readonly ILogger<AnalyzeNutrientDeficitHandler> _logger;
    private readonly IMapper _mapper;

    public AnalyzeNutrientDeficitHandler(
        IFertilizerCalculationService calculationService,
        IMapper mapper,
        ILogger<AnalyzeNutrientDeficitHandler> logger)
    {
        _calculationService = calculationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<NutrientDeficitAnalysisDto>> Handle(
        AnalyzeNutrientDeficitQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Аналіз дефіциту поживних речовин для поля {FieldId}", request.FieldId);

            var analysis = await _calculationService.AnalyzeNutrientDeficitAsync(request.FieldId);
            var analysisDto = _mapper.Map<NutrientDeficitAnalysisDto>(analysis);

            _logger.LogInformation(
                "Дефіцит поживних речовин проаналізовано для поля {FieldId}: Статус={Status}",
                request.FieldId,
                analysisDto.OverallStatus);

            return Result.Ok(analysisDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка аналізу дефіциту поживних речовин для поля {FieldId}", request.FieldId);
            return Result.Fail(new Error("Не вдалося проаналізувати дефіцит поживних речовин"));
        }
    }
}
