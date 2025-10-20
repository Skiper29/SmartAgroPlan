using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.SoilConditions;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Weather;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

public class CurrentRecommendationDto
{
    public int FieldId { get; set; }
    public DateTime GeneratedDate { get; set; }
    public string CurrentGrowthStage { get; set; }
    public int DaysAfterPlanting { get; set; }
    public bool ActionRequired { get; set; }
    public string Priority { get; set; }

    public NutrientRequirementDto RecommendedNutrients { get; set; }
    public List<FertilizerProductDto> RecommendedProducts { get; set; }

    public string Reasoning { get; set; }
    public List<string> Warnings { get; set; }
    public SoilConditionSummaryDto CurrentSoilCondition { get; set; }
    public WeatherImpactDto WeatherImpact { get; set; }
}
