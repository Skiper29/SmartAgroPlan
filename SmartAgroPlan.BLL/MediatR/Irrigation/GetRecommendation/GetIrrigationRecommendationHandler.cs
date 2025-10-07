using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using SmartAgroPlan.BLL.DTO.Irrigation;
using SmartAgroPlan.BLL.Interfaces.Irrigation;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;

public class GetIrrigationRecommendationHandler : IRequestHandler<GetIrrigationRecommendationCommand,
    Result<IrrigationRecommendationDto>>
{
    private readonly IFAO56Calculator _fao56Calculator;
    private readonly ILogger<GetIrrigationRecommendationHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IWeatherService _weatherService;

    public GetIrrigationRecommendationHandler(
        IRepositoryWrapper repositoryWrapper,
        ILogger<GetIrrigationRecommendationHandler> logger,
        IFAO56Calculator fao56Calculator,
        IWeatherService weatherService,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _fao56Calculator = fao56Calculator;
        _weatherService = weatherService;
        _mapper = mapper;
    }

    public async Task<Result<IrrigationRecommendationDto>> Handle(GetIrrigationRecommendationCommand request,
        CancellationToken cancellationToken)
    {
        var field = await _repositoryWrapper.FieldRepository
            .GetFirstOrDefaultAsync(f => f.Id == request.FieldId,
                f => f
                    .Include(e => e.CurrentCrop)
                    .Include(e => e.Soil)
                    .Include(e => e.Conditions)!);

        if (field == null)
        {
            var errorMsg = $"Не вдалося знайти поле з Id = {request.FieldId}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var definition = _repositoryWrapper.CropCoefficientDefinitionRepository
            .GetFirstOrDefaultAsync(ccd => ccd.CropType == field.CurrentCrop!.CropType).Result;

        if (definition == null)
        {
            var errorMsg =
                $"Не вдалося знайти визначення коефіцієнта культури для типу культури {field.CurrentCrop!.CropType}";
            _logger.LogError(errorMsg);
            throw new ArgumentException(errorMsg);
        }

        // Get field coordinates (assuming you have a method to extract from Polygon)
        var coords = GetFieldCoordinates(field.Boundary!
        );

        // Get current weather data
        var currentWeather = await _weatherService.GetCurrentWeatherAsync(
            coords.Latitude,
            coords.Longitude);

        // Get latest soil moisture if available
        var latestCondition = field.Conditions?
            .Where(c => c.SoilMoisture.HasValue)
            .OrderByDescending(c => c.RecordedAt)
            .FirstOrDefault();

        var soilMoisture = currentWeather.SoilMoisture;
        if (latestCondition != null && latestCondition.RecordedAt.Date > DateTime.UtcNow.Date.AddDays(-7))
            soilMoisture = latestCondition.SoilMoisture!.Value;

        // Calculate irrigation recommendation
        var recommendation = _fao56Calculator.CalculateIrrigationRequirement(
            field,
            definition,
            currentWeather,
            soilMoisture);

        var weatherDto = _mapper.Map<WeatherConditionsDto>(currentWeather);

        var recommendationDto = _mapper.Map<IrrigationRecommendationDto>(recommendation);
        recommendationDto.WeatherConditions = weatherDto;
        return Result.Ok(recommendationDto);
    }

    private (double Latitude, double Longitude) GetFieldCoordinates(Polygon boundary)
    {
        var centroid = boundary.Centroid;
        return (centroid.Y, centroid.X); // Latitude, Longitude
    }
}
