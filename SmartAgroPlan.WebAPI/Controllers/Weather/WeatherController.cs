using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.BLL.Models.Weather;

namespace SmartAgroPlan.WebAPI.Controllers.Weather;

[Route("api/[controller]")]
public class WeatherController : BaseApiController
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherData))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCurrentWeather(double lat, double lon)
    {
        var weather = await _weatherService.GetCurrentWeatherAsync(lat, lon);
        return Ok(weather);
    }

    [HttpGet("forecast")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WeatherData>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWeatherForecast(double lat, double lon, int days = 7)
    {
        var weather = await _weatherService.GetWeatherForecastAsync(lat, lon, days);
        return Ok(weather);
    }

    [HttpGet("historical")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WeatherData>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHistoricalWeather(double lat, double lon, DateTime startDate, DateTime endDate)
    {
        var weather = await _weatherService.GetHistoricalWeatherAsync(lat, lon, startDate, endDate);
        return Ok(weather);
    }
}
