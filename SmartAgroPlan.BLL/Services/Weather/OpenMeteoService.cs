using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartAgroPlan.BLL.Exceptions.WeatherExceptions;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.BLL.Models.Weather;

namespace SmartAgroPlan.BLL.Services.Weather;

public class OpenMeteoService : IWeatherService
{
    private const double SpeedConversionFactor = 3.6;
    private const double OpenMeteoDefaultHeight = 10.0;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenMeteoService> _logger;
    private readonly OpenMeteoOptions _options;

    public OpenMeteoService(HttpClient httpClient, ILogger<OpenMeteoService> logger, IOptions<OpenMeteoOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<WeatherData>> GetWeatherForecastAsync(double latitude, double longitude, int days = 7)
    {
        try
        {
            var url = BuildForecastUrl(latitude, longitude, days);
            var data = await GetFromApiAsync<OpenMeteoSimpleResponse>(url);

            return Enumerable.Range(0, days)
                .Select(i => MapToWeatherData(data, i))
                .ToList();
        }
        catch (Exception ex)
        {
            const string errorMsg = "Помилка під час отримання прогнозу погоди з Open-Meteо.";
            _logger.LogError(ex, errorMsg);
            throw new WeatherServiceException(errorMsg, ex);
        }
    }

    public async Task<WeatherData> GetCurrentWeatherAsync(double latitude, double longitude)
    {
        try
        {
            var url = BuildForecastUrl(latitude, longitude, 1, includeCurrent: true);
            var data = await GetFromApiAsync<OpenMeteoCurrentResponse>(url);
            return MapToWeatherData(data);
        }
        catch (Exception ex)
        {
            const string errorMsg = "Помилка під час отримання поточної погоди з Open-Meteo.";
            _logger.LogError(ex, errorMsg);
            throw new WeatherServiceException(errorMsg, ex);
        }
    }

    public async Task<List<WeatherData>> GetHistoricalWeatherAsync(double latitude, double longitude,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            var url = BuildForecastUrl(latitude, longitude, startDate: startDate, endDate: endDate);
            var data = await GetFromApiAsync<OpenMeteoSimpleResponse>(url);

            return data.Daily.Time
                .Select((_, i) => MapToWeatherData(data, i))
                .ToList();
        }
        catch (Exception ex)
        {
            const string errorMsg = "Помилка під час отримання історичних даних погоди з Open-Meteo.";
            _logger.LogError(ex, errorMsg);
            throw new WeatherServiceException(errorMsg, ex);
        }
    }

    private async Task<T> GetFromApiAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions)!;
    }

    private string GetCoordsString(double latitude, double longitude)
    {
        return
            $"latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";
    }

    private string BuildForecastUrl(
        double latitude,
        double longitude,
        int? forecastDays = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool includeCurrent = false)
    {
        var sb = new StringBuilder($"{_options.BaseUrl}/forecast?");
        sb.Append(GetCoordsString(latitude, longitude));
        sb.Append($"&hourly={_options.HourlyParams}&daily={_options.DailyParams}&timezone=auto");

        if (forecastDays.HasValue)
            sb.Append($"&forecast_days={forecastDays.Value}");
        if (startDate.HasValue && endDate.HasValue)
            sb.Append($"&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}");
        if (includeCurrent)
            sb.Append($"&current={_options.HourlyParams}");

        return sb.ToString();
    }

    private static double ConvertWindSpeedTo2M(double uInKmh, double z0Meters = 0.03)
    {
        // Convert km/h to m/s
        var uInMs = uInKmh / SpeedConversionFactor;

        // Apply logarithmic wind profile correction to 2m height
        var ratio = Math.Log(2.0 / z0Meters) / Math.Log(OpenMeteoDefaultHeight / z0Meters);

        // Corrected wind speed at 2m height
        var u2InMs = uInMs * ratio;

        return u2InMs;
    }

    private static WeatherData MapToWeatherData(OpenMeteoSimpleResponse response, int dayIndex)
    {
        var startHour = dayIndex * 24;

        double Average(IReadOnlyList<double> list)
        {
            return list.Count == 0 ? 0 : list.Average();
        }

        var hourlyTemp = response.Hourly.Temperature_2m.Skip(startHour).Take(24).ToList();
        var hourlyHumidity = response.Hourly.RelativeHumidity_2m.Skip(startHour).Take(24).ToList();
        var hourlyWind = response.Hourly.WindSpeed_10m.Skip(startHour).Take(24).ToList();
        var hourlyPressure = response.Hourly.Surface_Pressure.Skip(startHour).Take(24).ToList();
        var hourlySoilMoisture = response.Hourly.Soil_Moisture_3_To_9cm.Skip(startHour).Take(24).ToList();

        return new WeatherData
        {
            Date = DateTime.Parse(response.Daily.Time[dayIndex]),
            Temperature = Average(hourlyTemp),
            MinTemperature = response.Daily.Temperature_2m_min[dayIndex],
            MaxTemperature = response.Daily.Temperature_2m_max[dayIndex],
            RelativeHumidity = Average(hourlyHumidity),
            WindSpeed = ConvertWindSpeedTo2M(Average(hourlyWind)),
            AtmosphericPressure = Average(hourlyPressure) / 1000, // Pa → kPa
            SoilMoisture = Average(hourlySoilMoisture),
            Precipitation = response.Daily.Precipitation_sum[dayIndex],
            SolarRadiation = response.Daily.Shortwave_radiation_sum[dayIndex],
            Elevation = response.Elevation
        };
    }

    private static WeatherData MapToWeatherData(OpenMeteoCurrentResponse currentResponse)
    {
        return new WeatherData
        {
            Date = DateTime.Parse(currentResponse.Current.Time),
            Temperature = currentResponse.Current.Temperature_2m,
            Elevation = currentResponse.Elevation,
            RelativeHumidity = currentResponse.Current.RelativeHumidity_2m,
            WindSpeed = ConvertWindSpeedTo2M(currentResponse.Current.WindSpeed_10m),
            AtmosphericPressure = currentResponse.Current.Surface_Pressure / 1000, // Convert Pa to kPa
            SoilMoisture = currentResponse.Current.Soil_Moisture_3_To_9cm,
            MinTemperature = currentResponse.Daily.Temperature_2m_min.FirstOrDefault(),
            MaxTemperature = currentResponse.Daily.Temperature_2m_max.FirstOrDefault(),
            Precipitation = currentResponse.Daily.Precipitation_sum.FirstOrDefault(),
            SolarRadiation =
                currentResponse.Daily.Shortwave_radiation_sum.FirstOrDefault() // MJ/m²/day
        };
    }
}
