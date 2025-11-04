namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

/// <summary>
///     Detailed analysis of nutrient balance for a field
/// </summary>
public class NutrientBalance
{
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
    public string? CropName { get; set; }
    public DateTime AnalysisDate { get; set; }
    public int DaysAfterPlanting { get; set; }
    public int DaysToHarvest { get; set; }

    // Nutrient status
    public NutrientRequirement RequiredForTargetYield { get; set; } = null!;
    public NutrientRequirement AvailableInSoil { get; set; } = null!;
    public NutrientRequirement AlreadyApplied { get; set; } = null!;
    public NutrientRequirement Deficit { get; set; } = null!;
    public NutrientRequirement Surplus { get; set; } = null!;

    // Recommendations
    public string OverallStatus { get; set; } = null!; // Optimal, Deficient, Excessive
    public List<string> Recommendations { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
