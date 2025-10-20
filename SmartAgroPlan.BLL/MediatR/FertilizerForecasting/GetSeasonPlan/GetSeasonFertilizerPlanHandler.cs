using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.GetSeasonPlan;

public class
    GetSeasonFertilizerPlanHandler : IRequestHandler<GetSeasonFertilizerPlanQuery, Result<SeasonFertilizerPlanDto>>
{
    private readonly IFertilizerCalculationService _fertilizerCalculationService;
    private readonly ILogger<GetSeasonFertilizerPlanHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetSeasonFertilizerPlanHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IFertilizerCalculationService fertilizerCalculationService,
        ILogger<GetSeasonFertilizerPlanHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _fertilizerCalculationService = fertilizerCalculationService;
        _logger = logger;
    }

    public async Task<Result<SeasonFertilizerPlanDto>> Handle(
        GetSeasonFertilizerPlanQuery request,
        CancellationToken cancellationToken)
    {
        var field = await _repositoryWrapper.FieldRepository.GetFirstOrDefaultAsync(
            f => f.Id == request.FieldId,
            f => f
                .Include(e => e.CurrentCrop)
                .Include(e => e.Soil)
                .Include(e => e.Conditions!.OrderByDescending(c => c.RecordedAt).Take(1))
        );

        if (field is null)
        {
            var errorMsg = $"Поле с ID {request.FieldId} не найдено.";
            _logger.LogWarning(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var fieldAreaDto = _mapper.Map<FieldWithAreaDto>(field);

        var area = await _repositoryWrapper.FieldRepository.FindAll(f => f.Id == request.FieldId)
            .Select(f => f.Boundary!.Area / 10000.0)
            .FirstOrDefaultAsync(cancellationToken);

        fieldAreaDto.AreaInHectares = area;

        var sowingDate = request.PlannedSowingDate ?? field.SowingDate;

        if (sowingDate is null)
        {
            var errorMsg = $"Дата посіву не указана для поля с ID {request.FieldId}.";
            _logger.LogWarning(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var latestCondition = field.Conditions?.FirstOrDefault();
        if (latestCondition is null)
        {
            var errorMsg = $"Немає даних про стан ґрунту для поля з ID {request.FieldId}.";
            _logger.LogWarning(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        // Determine target yield
        var targetYield = request.TargetYieldOverride ?? field.CurrentCrop!.HarvestYield;

        // Calculate the season fertilizer plan
        var seasonPlan = _fertilizerCalculationService.CalculateSeasonPlan(
            field.CurrentCrop!,
            field.Soil!,
            latestCondition,
            fieldAreaDto,
            targetYield,
            sowingDate.Value
        );

        var seasonPlanDto = _mapper.Map<SeasonFertilizerPlanDto>(seasonPlan);
        seasonPlanDto.FieldName = field.Name;
        seasonPlanDto.SowingDate = sowingDate.Value;
        seasonPlanDto.ExpectedHarvestDate = sowingDate.Value.AddDays(field.CurrentCrop!.GrowingDuration);

        return Result.Ok(seasonPlanDto);
    }
}
