namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

/// <summary>
///     Represents a summary of nutrient applications for a specific date range
/// </summary>
public class NutrientApplicationSummary
{
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public NutrientRequirement TotalApplied { get; set; } = null!;
    public NutrientRequirement PlannedToApply { get; set; } = null!;
    public int CompletedApplications { get; set; }
    public int PendingApplications { get; set; }
    public List<ApplicationSummaryItem> Applications { get; set; } = new();
}
