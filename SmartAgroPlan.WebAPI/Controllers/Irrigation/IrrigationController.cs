using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.Irrigation;
using SmartAgroPlan.BLL.MediatR.Irrigation.GetBatchRecommendations;
using SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;
using SmartAgroPlan.BLL.MediatR.Irrigation.GetSchedule;

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

    /// <summary>
    ///     Get irrigation recommendations for multiple fields
    /// </summary>
    [HttpPost("recommendations/batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBatchIrrigationRecommendations(
        [FromBody] GetBatchIrrigationRecommendationsCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    /// <summary>
    ///     Get weekly irrigation schedule for a field
    /// </summary>
    [HttpGet("schedule/weekly/{fieldId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeeklyIrrigationScheduleDto>> GetWeeklySchedule(
        int fieldId,
        [FromQuery] DateTime? startDate = null)
    {
        return HandleResult(await Mediator.Send(new GetWeeklyIrrigationScheduleCommand(fieldId, startDate)));
    }
}
