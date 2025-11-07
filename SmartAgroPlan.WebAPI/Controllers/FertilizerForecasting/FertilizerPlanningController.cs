using Microsoft.AspNetCore.Mvc;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Analysis.AnalyzeNutrientDeficit;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Analysis.GetNutrientBalance;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetApplicationsForDateRange;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetApplicationSummary;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.GetUpcomingApplications;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.RecordApplication;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.CurrentRecommendation;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.SeasonPlan.CalculateSeasonPlan;
using SmartAgroPlan.BLL.MediatR.FertilizerForecasting.SeasonPlan.SaveSeasonPlan;

namespace SmartAgroPlan.WebAPI.Controllers.FertilizerForecasting;

public class FertilizerPlanningController : BaseApiController
{
    /// <summary>
    ///     Calculate complete season fertilizer plan for a field
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SeasonFertilizerPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateSeasonPlan(
        [FromQuery] int fieldId,
        [FromQuery] double? targetYield = null,
        [FromQuery] DateTime? sowingDate = null)
    {
        return HandleResult(await Mediator.Send(new CalculateSeasonPlanQuery(
            fieldId,
            targetYield ?? 0,
            sowingDate)));
    }

    /// <summary>
    ///     Calculate and save season fertilizer plan to database
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveSeasonPlan([FromBody] SaveSeasonPlanRequest request)
    {
        return HandleResult(await Mediator.Send(new SaveSeasonPlanCommand(
            request.FieldId,
            request.TargetYield,
            request.SowingDate)));
    }

    /// <summary>
    ///     Get current fertilizer recommendation based on field status
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CurrentRecommendationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentRecommendation([FromQuery] int fieldId)
    {
        return HandleResult(await Mediator.Send(new GetCurrentRecommendationQuery(fieldId)));
    }

    /// <summary>
    ///     Get upcoming fertilizer applications for the next N days
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FertilizerApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingApplications([FromQuery] int fieldId, [FromQuery] int daysAhead = 14)
    {
        return HandleResult(await Mediator.Send(new GetUpcomingApplicationsQuery(fieldId, daysAhead)));
    }

    /// <summary>
    ///     Get fertilizer applications within a specific date range
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FertilizerApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationsByDateRange(
        [FromQuery] int fieldId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        return HandleResult(await Mediator.Send(new GetApplicationsForDateRangeQuery(fieldId, startDate, endDate)));
    }

    /// <summary>
    ///     Get comprehensive nutrient balance analysis for a field
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(NutrientBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNutrientBalance([FromQuery] int fieldId)
    {
        return HandleResult(await Mediator.Send(new GetNutrientBalanceQuery(fieldId)));
    }

    /// <summary>
    ///     Analyze nutrient deficits and get urgency assessment
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> AnalyzeNutrientDeficit([FromQuery] int fieldId)
    {
        return HandleResult(await Mediator.Send(new AnalyzeNutrientDeficitQuery(fieldId)));
    }

    /// <summary>
    ///     Get application summary for a date range
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(NutrientApplicationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationSummary(
        [FromQuery] int fieldId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        return HandleResult(await Mediator.Send(new GetApplicationSummaryQuery(fieldId, fromDate, toDate)));
    }

    /// <summary>
    ///     Record an actual fertilizer application
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordApplication([FromBody] RecordApplicationRequest request)
    {
        return HandleResult(await Mediator.Send(new RecordApplicationCommand(
            request.FieldId,
            request.ApplicationDate,
            request.ApplicationMethodId,
            request.ProductsUsed,
            request.ApplicationPlanId,
            request.Notes,
            request.Temperature,
            request.WindSpeed,
            request.Humidity)));
    }
}

// Request DTOs
public record SaveSeasonPlanRequest(int FieldId, double TargetYield, DateTime? SowingDate);

public record RecordApplicationRequest(
    int FieldId,
    DateTime ApplicationDate,
    int ApplicationMethodId,
    Dictionary<int, double> ProductsUsed,
    int? ApplicationPlanId = null,
    string? Notes = null,
    double? Temperature = null,
    double? WindSpeed = null,
    double? Humidity = null);