using SmartAgroPlan.BLL.Models.FertilizerForecasting;

namespace SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

/// <summary>
///     Service for managing fertilizer application plans (CRUD operations)
/// </summary>
public interface IFertilizerPlanManagementService
{
    /// <summary>
    ///     Saves a calculated season plan to database
    /// </summary>
    Task<int> SaveSeasonPlanAsync(SeasonFertilizerPlan plan, int fieldId);

    /// <summary>
    ///     Gets saved application plans for a field
    /// </summary>
    Task<List<FertilizerApplication>> GetSavedApplicationPlansAsync(int fieldId, bool includeCompleted = false);

    /// <summary>
    ///     Gets upcoming applications for the next N days
    /// </summary>
    Task<List<FertilizerApplication>> GetUpcomingApplicationsAsync(int fieldId, int daysAhead = 14);

    /// <summary>
    ///     Updates an application plan
    /// </summary>
    Task UpdateApplicationPlanAsync(int applicationPlanId, FertilizerApplication updatedApplication);

    /// <summary>
    ///     Marks an application as completed
    /// </summary>
    Task MarkApplicationCompletedAsync(int applicationPlanId, DateTime actualDate);

    /// <summary>
    ///     Deletes an application plan
    /// </summary>
    Task DeleteApplicationPlanAsync(int applicationPlanId);

    /// <summary>
    ///     Gets application summary for a date range
    /// </summary>
    Task<NutrientApplicationSummary> GetApplicationSummaryAsync(int fieldId, DateTime fromDate, DateTime toDate);
}