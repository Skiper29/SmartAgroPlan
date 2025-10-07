using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.BLL.Models.Weather;

namespace SmartAgroPlan.BLL.Services.Weather;

public class OpenMeteoService : IWeatherService
{
    private const string BaseUrl = "https://api.open-meteo.com/v1";

    private const string DailyParams =
        "temperature_2m_max,temperature_2m_min,precipitation_sum,shortwave_radiation_sum";

    private const string HourlyParams =
        "temperature_2m,relativehumidity_2m,windspeed_10m,surface_pressure";

    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenMeteoService> _logger;

    public OpenMeteoService(HttpClient httpClient, ILogger<OpenMeteoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherData> GetCurrentWeatherAsync(double latitude, double longitude)
    {
        try
        {
            var url = $"{BaseUrl}/forecast?" +
                      GetCordsString(latitude, longitude) +
                      $"&current={HourlyParams}" +
                      $"&daily={DailyParams}" +
                      "&timezone=auto&forecast_days=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenMeteoCurrentResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return ConvertToCurrentWeatherData(data!);
        }
        catch (Exception ex)
        {
            var errorMsg = "Помилка під час отримання поточної погоди з Open-Meteo";
            _logger.LogError(ex, errorMsg);
            throw new ApplicationException(errorMsg, ex);
        }
    }

    public async Task<List<WeatherData>> GetWeatherForecastAsync(double latitude, double longitude, int days = 7)
    {
        try
        {
            var url = $"{BaseUrl}/forecast?" +
                      GetCordsString(latitude, longitude) +
                      $"&hourly={HourlyParams}" +
                      $"&daily={DailyParams}" +
                      $"&forecast_days={days}" +
                      "&timezone=auto";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenMeteoSimpleResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var weatherList = new List<WeatherData>();
            for (var i = 0; i < days; i++) weatherList.Add(ConvertToWeatherData(data!, i));

            return weatherList;
        }
        catch (Exception ex)
        {
            var errorMsg = "Помилка під час отримання прогнозу погоди з Open-Meteo";
            _logger.LogError(ex, errorMsg);
            throw new ApplicationException(errorMsg, ex);
        }
    }

    public async Task<List<WeatherData>> GetHistoricalWeatherAsync(double latitude, double longitude,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            var url = $"{BaseUrl}/forecast?" +
                      GetCordsString(latitude, longitude) +
                      $"&start_date={startDate:yyyy-MM-dd}" +
                      $"&end_date={endDate:yyyy-MM-dd}" +
                      $"&hourly={HourlyParams}" +
                      $"&daily={DailyParams}" +
                      "&timezone=auto";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<OpenMeteoSimpleResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var historicalData = new List<WeatherData>();
            for (var i = 0; i < data!.Daily.Time.Count; i++) historicalData.Add(ConvertToWeatherData(data, i));

            return historicalData;
        }
        catch (Exception e)
        {
            var errorMsg = "Помилка під час отримання історичних даних погоди з Open-Meteo";
            _logger.LogError(e, errorMsg);
            throw new ApplicationException(errorMsg, e);
        }
    }

    private string GetCordsString(double latitude, double longitude)
    {
        return
            $"latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";
    }

    private WeatherData ConvertToWeatherData(OpenMeteoSimpleResponse simpleResponse, int dayIndex)
    {
        // Calculate daily averages from hourly data
        var startHour = dayIndex * 24;

        var hourlyTemp = simpleResponse.Hourly.Temperature_2m.Skip(startHour).Take(24).ToList();
        var hourlyHumidity = simpleResponse.Hourly.RelativeHumidity_2m.Skip(startHour).Take(24).ToList();
        var hourlyWind = simpleResponse.Hourly.WindSpeed_10m.Skip(startHour).Take(24).ToList();
        var hourlyPressure = simpleResponse.Hourly.Surface_Pressure.Skip(startHour).Take(24).ToList();

        return new WeatherData
        {
            Date = DateTime.Parse(simpleResponse.Daily.Time[dayIndex]),
            Temperature = hourlyTemp.Any() ? hourlyTemp.Average() : 0,
            MinTemperature = simpleResponse.Daily.Temperature_2m_min[dayIndex],
            MaxTemperature = simpleResponse.Daily.Temperature_2m_max[dayIndex],
            RelativeHumidity = hourlyHumidity.Any() ? hourlyHumidity.Average() : 0,
            WindSpeed = hourlyWind.Any() ? hourlyWind.Average() : 0,
            Precipitation = simpleResponse.Daily.Precipitation_sum[dayIndex],
            SolarRadiation = simpleResponse.Daily.Shortwave_radiation_sum[dayIndex], //MJ/m²/day
            AtmosphericPressure = hourlyPressure.Any() ? hourlyPressure.Average() / 1000 : null // Convert Pa to kPa
        };
    }

    private WeatherData ConvertToCurrentWeatherData(OpenMeteoCurrentResponse currentResponse)
    {
        return new WeatherData
        {
            Date = DateTime.Parse(currentResponse.Current.Time),
            Temperature = currentResponse.Current.Temperature_2m,
            RelativeHumidity = currentResponse.Current.RelativeHumidity_2m,
            WindSpeed = currentResponse.Current.WindSpeed_10m,
            AtmosphericPressure = currentResponse.Current.Surface_Pressure / 1000, // Convert Pa to kPa
            MinTemperature = currentResponse.Daily.Temperature_2m_min.FirstOrDefault(),
            MaxTemperature = currentResponse.Daily.Temperature_2m_max.FirstOrDefault(),
            Precipitation = currentResponse.Daily.Precipitation_sum.FirstOrDefault(),
            SolarRadiation =
                currentResponse.Daily.Shortwave_radiation_sum.FirstOrDefault() // MJ/m²/day
        };
    }
}
