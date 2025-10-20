using System.Text;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Irrigation.Schedule;
using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.BLL.Interfaces.Irrigation;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.BLL.Utils;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetSchedule;

public class
    GetWeeklyIrrigationScheduleHandler : IRequestHandler<GetWeeklyIrrigationScheduleCommand,
    Result<WeeklyIrrigationScheduleDto>>
{
    private const double GrossToNetIrrigationEfficiency = 0.85; // Efficiency factor for gross to net irrigation
    private const double IrrigationThreshold = 2.0; // Threshold in mm to decide if irrigation is needed
    private const double MaxDailyIrrigation = 20.0; // Maximum daily irrigation in mm to prevent over-irrigation

    private readonly IFAO56Calculator _calculator;
    private readonly ICropCoefficientService _cropCoefficientService;
    private readonly ILogger<GetWeeklyIrrigationScheduleHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IWeatherService _weatherService;

    public GetWeeklyIrrigationScheduleHandler(
        IRepositoryWrapper repositoryWrapper,
        IFAO56Calculator calculator,
        IWeatherService weatherService,
        ICropCoefficientService cropCoefficientService,
        ILogger<GetWeeklyIrrigationScheduleHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _calculator = calculator;
        _weatherService = weatherService;
        _cropCoefficientService = cropCoefficientService;
        _logger = logger;
    }

    public async Task<Result<WeeklyIrrigationScheduleDto>> Handle(GetWeeklyIrrigationScheduleCommand request,
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
            return Fail(errorMsg);
        }

        if (field.CurrentCrop == null)
        {
            var errorMsg = $"На полі з Id = {request.FieldId} не встановлена культура";
            return Fail(errorMsg);
        }

        if (field.Soil == null)
        {
            var errorMsg = $"На полі з Id = {request.FieldId} не встановлено тип ґрунту";
            return Fail(errorMsg);
        }

        var definition = await _repositoryWrapper.CropCoefficientDefinitionRepository
            .GetFirstOrDefaultAsync(ccd => ccd.CropType == field.CurrentCrop.CropType);

        if (definition == null)
        {
            var errorMsg =
                $"Не вдалося знайти визначення коефіцієнта культури для типу культури {field.CurrentCrop.CropType}";
            return Fail(errorMsg);
        }

        var coords = field.Boundary!.Centroid;
        var startDate = request.StartDate ?? DateTime.Today;

        var forecasts = await _weatherService.GetWeatherForecastAsync(
            coords.Y,
            coords.X);

        var schedule = new WeeklyIrrigationScheduleDto
        {
            FieldId = field.Id,
            FieldName = field.Name!,
            CropType = field.CurrentCrop!.CropType.ToString(),
            StartDate = startDate,
            EndDate = startDate.AddDays(6),
            DailySchedule = new List<DailyIrrigationDto>(),
            TotalWaterRequirement = 0,
            TotalExpectedPrecipitation = 0
        };

        var soilMoisture = SoilMoistureHelper.GetSoilMoisture(field.Conditions, forecasts[0]);

        foreach (var weather in forecasts)
        {
            var rec = _calculator.CalculateIrrigationRequirement(field, definition, weather, soilMoisture);

            if (rec.GrossIrrigationRequirement > MaxDailyIrrigation)
                rec.GrossIrrigationRequirement = MaxDailyIrrigation; // Cap to avoid excessive irrigation

            schedule.DailySchedule.Add(new DailyIrrigationDto
            {
                Date = weather.Date,
                DayOfWeek = weather.Date.DayOfWeek.ToString(),
                ET0 = rec.ET0,
                ETc = rec.ETc,
                Precipitation = weather.Precipitation,
                NetIrrigationRequired = rec.NetIrrigationRequirement,
                GrossIrrigationRequired = rec.GrossIrrigationRequirement,
                SoilMoisture = soilMoisture,
                ShouldIrrigate = rec.GrossIrrigationRequirement > IrrigationThreshold, // Threshold
                RecommendedTime = GetOptimalIrrigationTime(weather),
                WeatherSummary = GetWeatherSummary(weather)
            });

            schedule.TotalWaterRequirement += rec.GrossIrrigationRequirement;
            schedule.TotalExpectedPrecipitation += weather.Precipitation;

            soilMoisture = SoilMoistureHelper.UpdateSoilMoisture(
                soilMoisture,
                rec.ET0,
                weather.Precipitation,
                rec.GrossIrrigationRequirement / GrossToNetIrrigationEfficiency);
        }

        schedule.IrrigationDays = schedule.DailySchedule.Count(d => d.ShouldIrrigate);
        schedule.Recommendations = GenerateWeeklyRecommendations(schedule);

        return schedule;
    }

    private string GenerateWeeklyRecommendations(WeeklyIrrigationScheduleDto schedule)
    {
        var recommendations = new StringBuilder();

        if (schedule.TotalWaterRequirement > 100)
            recommendations.AppendLine("Цього тижня потрібно багато води, переконайтеся, що система зрошення справна.");
        else if (schedule.TotalWaterRequirement > 50)
            recommendations.AppendLine("Помірна потреба у воді. Плануйте зрошення відповідно.");

        if (schedule.TotalExpectedPrecipitation > 20)
            recommendations.AppendLine("Очікується значна кількість опадів, можливо, зрошення не знадобиться.");

        if (schedule.IrrigationDays > 5)
            recommendations.AppendLine("Часте зрошення може призвести до заболочування. Слідкуйте за станом ґрунту.");

        var hotDays = schedule.DailySchedule.Count(d =>
            d.WeatherSummary.Contains("Жарко") || d.WeatherSummary.Contains("Дуже спекотно"));
        if (hotDays > 3)
            recommendations.AppendLine(
                "Цього тижня очікується багато спекотних днів. Зверніть увагу на потребу у воді.");

        return recommendations.ToString().Trim();
    }

    private string GetWeatherSummary(WeatherData weather)
    {
        var conditions = new StringBuilder();

        if (weather.MaxTemperature > 35) conditions.AppendLine("Дуже спекотно");
        else if (weather.MaxTemperature > 28) conditions.AppendLine("Жарко");
        else if (weather.MaxTemperature > 20) conditions.AppendLine("Тепло");
        else conditions.AppendLine("Помірно");

        if (weather.Precipitation > 10) conditions.AppendLine("Сильний дощ");
        else if (weather.Precipitation > 2) conditions.AppendLine("Легкий дощ");
        else conditions.AppendLine("Без опадів");

        conditions.AppendLine(weather.WindSpeed > 5 ? "Вітряно" : "Спокійно");

        return conditions.ToString().Trim();
    }

    private string GetOptimalIrrigationTime(WeatherData weather)
    {
        if (weather.MaxTemperature > 30) return "Рано вранці (5-7 година) або ввечері (19-21 година)";

        return weather.WindSpeed > 5
            ? "Рано вранці коли вітер слабкий або оберіть інший день"
            : "Рано вранці (5-10 година) для кращої ефективності";
    }

    private Result Fail(string errorMsg)
    {
        _logger.LogError(errorMsg);
        return Result.Fail(new Error(errorMsg));
    }
}
