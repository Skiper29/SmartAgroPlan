using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

public class SeasonFertilizerPlanDto
{
    public int FieldId { get; set; }
    public string? CropName { get; set; }
    public string? FieldName { get; set; }
    public DateTime SowingDate { get; set; }
    public DateTime ExpectedHarvestDate { get; set; }
    public DateTime PlanGeneratedDate { get; set; }

    // Nutrient calculations
    public NutrientRequirementDto TotalSeasonRequirement { get; set; } = null!;
    public NutrientRequirementDto SoilSupply { get; set; } = null!;
    public NutrientRequirementDto RequiredFromFertilizer { get; set; } = null!;
    public NutrientRequirementDto AlreadyApplied { get; set; } = null!;
    public NutrientRequirementDto RemainingToApply { get; set; } = null!;

    // Application schedule
    public List<FertilizerApplicationDto> Applications { get; set; } = new();

    // Field information
    public double FieldAreaHa { get; set; }
    public double ExpectedYield { get; set; }

    // Status
    public string? Notes { get; set; }
    public bool IsSaved { get; set; }
    public int? SavedPlanId { get; set; }
}