using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.FertilizerForecasting.Nutrients;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

/// <summary>
///     Service for calculating fertilizer requirements based on crop needs, soil conditions,
///     and growth stages following precision agriculture best practices
/// </summary>
public interface IFertilizerCalculationService
{
    /// <summary>
    ///     Calculates complete season fertilizer plan from current date to harvest
    ///     Based on yield targeting approach and growth stage timing
    /// </summary>
    Task<SeasonFertilizerPlan> CalculateSeasonPlanAsync(
        int fieldId,
        double targetYield,
        DateTime? customSowingDate = null);

    /// <summary>
    ///     Gets current recommendation for immediate application based on:
    ///     - Current growth stage
    ///     - Soil nutrient status
    ///     - Weather conditions
    ///     - Days since planting
    /// </summary>
    Task<CurrentRecommendation> GetCurrentRecommendationAsync(int fieldId);

    /// <summary>
    ///     Calculates fertilizer plan for specific date range
    ///     Useful for planning applications for next 2-4 weeks
    /// </summary>
    Task<List<FertilizerApplication>> GetApplicationsForDateRangeAsync(
        int fieldId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    ///     Analyzes current nutrient deficit based on soil test and crop requirements
    ///     Provides urgency assessment and recommendations
    /// </summary>
    Task<NutrientDeficitAnalysis> AnalyzeNutrientDeficitAsync(int fieldId);

    /// <summary>
    ///     Gets comprehensive nutrient balance analysis for a field
    /// </summary>
    Task<NutrientBalance> GetNutrientBalanceAsync(int fieldId);

    /// <summary>
    ///     Calculates total nutrients required for target yield
    /// </summary>
    NutrientRequirement CalculateTotalNutrientRequirement(
        CropVariety crop,
        double targetYield,
        double fieldAreaHa);

    /// <summary>
    ///     Estimates nutrient supply from soil based on test results
    /// </summary>
    NutrientRequirement CalculateSoilNutrientSupply(
        Soil soil,
        FieldCondition latestCondition,
        CropVariety crop);

    /// <summary>
    ///     Optimizes product selection to meet nutrient requirements
    /// </summary>
    Task<ProductRecommendation> OptimizeProductSelectionAsync(
        NutrientRequirement targetNutrients,
        double fieldAreaHa,
        string optimizationStrategy = "Balanced");
}
