using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

public class NutrientBalanceDto
{
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
    public string? CropName { get; set; }
    public DateTime AnalysisDate { get; set; }
    public int DaysAfterPlanting { get; set; }
    public int DaysToHarvest { get; set; }

    public NutrientRequirementDto RequiredForTargetYield { get; set; } = null!;
    public NutrientRequirementDto AvailableInSoil { get; set; } = null!;
    public NutrientRequirementDto AlreadyApplied { get; set; } = null!;
    public NutrientRequirementDto Deficit { get; set; } = null!;
    public NutrientRequirementDto Surplus { get; set; } = null!;
    public string OverallStatus { get; set; } = null!;
    public List<string> Recommendations { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
