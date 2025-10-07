using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;

namespace SmartAgroPlan.WebAPI.Controllers.Irrigation;

[Route("api/[controller]")]
public class IrrigationController : BaseApiController
{
    /// <summary>
    ///     Get irrigation recommendation for a specific field
    /// </summary>
    [HttpGet("recommendation/{fieldId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetIrrigationRecommendation(
        int fieldId,
        [FromQuery] bool includeForecast = false,
        [FromQuery] int forecastDays = 7)
    {
        return HandleResult(await Mediator.Send(new GetIrrigationRecommendationCommand
        {
            FieldId = fieldId,
            IncludeForecast = includeForecast,
            ForecastDays = forecastDays
        }));
    }
}
