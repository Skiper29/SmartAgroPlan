using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

public class SeasonFertilizerPlanDto
{
    public int FieldId { get; set; }
    public string FieldName { get; set; }
    public string CropName { get; set; }
    public double FieldAreaHa { get; set; }
    public DateTime SowingDate { get; set; }
    public DateTime ExpectedHarvestDate { get; set; }
    public double ExpectedYield { get; set; }

    public NutrientRequirementDto TotalSeasonRequirement { get; set; }
    public NutrientRequirementDto SoilSupply { get; set; }
    public NutrientRequirementDto RequiredFromFertilizer { get; set; }

    public List<FertilizerApplicationDto> PlannedApplications { get; set; }
    public FertilizerCostEstimateDto CostEstimate { get; set; }
    public string Notes { get; set; }
}
