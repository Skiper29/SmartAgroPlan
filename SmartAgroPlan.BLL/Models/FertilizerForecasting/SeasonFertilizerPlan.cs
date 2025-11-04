namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class SeasonFertilizerPlan
{
    public int FieldId { get; set; }
    public string? CropName { get; set; }
    public string? FieldName { get; set; }
    public DateTime SowingDate { get; set; }
    public DateTime ExpectedHarvestDate { get; set; }
    public DateTime PlanGeneratedDate { get; set; }

    // Nutrient calculations
    public NutrientRequirement TotalSeasonRequirement { get; set; } = null!;
    public NutrientRequirement SoilSupply { get; set; } = null!;
    public NutrientRequirement RequiredFromFertilizer { get; set; } = null!;
    public NutrientRequirement AlreadyApplied { get; set; } = null!;
    public NutrientRequirement RemainingToApply { get; set; } = null!;

    // Application schedule
    public List<FertilizerApplication> Applications { get; set; } = new();

    // Field information
    public double FieldAreaHa { get; set; }
    public double ExpectedYield { get; set; }

    // Status
    public string? Notes { get; set; }
    public bool IsSaved { get; set; }
    public int? SavedPlanId { get; set; }
}
