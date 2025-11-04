using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Analysis.GetNutrientBalance;

public class GetNutrientBalanceHandler : IRequestHandler<GetNutrientBalanceQuery, Result<NutrientBalanceDto>>
{
    private readonly IFertilizerCalculationService _calculationService;
    private readonly ILogger<GetNutrientBalanceHandler> _logger;
    private readonly IMapper _mapper;

    public GetNutrientBalanceHandler(
        IFertilizerCalculationService calculationService,
        IMapper mapper,
        ILogger<GetNutrientBalanceHandler> logger)
    {
        _calculationService = calculationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<NutrientBalanceDto>> Handle(
        GetNutrientBalanceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Отримання балансу поживних речовин для поля {FieldId}", request.FieldId);

            var balance = await _calculationService.GetNutrientBalanceAsync(request.FieldId);
            var balanceDto = _mapper.Map<NutrientBalanceDto>(balance);

            _logger.LogInformation(
                "Баланс поживних речовин отримано для поля {FieldId}: Статус={Status}",
                request.FieldId,
                balanceDto.OverallStatus);

            return Result.Ok(balanceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка отримання балансу поживних речовин для поля {FieldId}", request.FieldId);
            return Result.Fail(new Error("Не вдалося отримати баланс поживних речовин"));
        }
    }
}
