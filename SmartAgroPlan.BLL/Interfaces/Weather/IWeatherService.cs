using SmartAgroPlan.BLL.Models.Weather;

namespace SmartAgroPlan.BLL.Interfaces.Weather;

public interface IWeatherService
{
    Task<WeatherData> GetCurrentWeatherAsync(double latitude, double longitude);
    Task<List<WeatherData>> GetWeatherForecastAsync(double latitude, double longitude, int days = 7);

    Task<List<WeatherData>> GetHistoricalWeatherAsync(double latitude, double longitude, DateTime startDate,
        DateTime endDate);
}
