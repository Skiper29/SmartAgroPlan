using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.Interfaces.Weather;
using SmartAgroPlan.BLL.Models.Weather;

namespace SmartAgroPlan.WebAPI.Controllers.Irrigation;

public class IrrigationController : BaseApiController
{
    private readonly IWeatherService _weatherService;

    public IrrigationController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("weather/forecast")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WeatherData))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWeatherForecast(double lat, double lon)
    {
        var weather = await _weatherService.GetCurrentWeatherAsync(lat, lon);
        return Ok(weather);
    }
}
