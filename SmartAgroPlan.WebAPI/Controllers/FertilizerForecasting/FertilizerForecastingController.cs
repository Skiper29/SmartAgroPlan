using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.GetSeasonPlan;

namespace SmartAgroPlan.WebAPI.Controllers.FertilizerForecasting;

public class FertilizerForecastingController : BaseApiController
{
    /// <summary>
    ///     Get complete season fertilizer plan for a field
    /// </summary>
    /// <param name="fieldId">Field ID</param>
    /// <param name="targetYield">Optional: Override target yield (tonnes/ha)</param>
    /// <param name="plannedSowingDate">Optional: Planned sowing date</param>
    /// <returns>Complete fertilizer plan for the growing season</returns>
    [HttpGet("season-plan/{fieldId}")]
    [ProducesResponseType(typeof(SeasonFertilizerPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeasonFertilizerPlanDto>> GetSeasonPlan(
        int fieldId,
        [FromQuery] double? targetYield = null,
        [FromQuery] DateTime? plannedSowingDate = null)
    {
        var query = new GetSeasonFertilizerPlanQuery(
            fieldId,
            targetYield,
            plannedSowingDate);
        return HandleResult(await Mediator.Send(query));
    }
}
