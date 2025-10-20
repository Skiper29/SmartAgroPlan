using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;

public class FertilizerApplicationDto
{
    public int? Id { get; set; }
    public DateTime RecommendedDate { get; set; }
    public string CropStage { get; set; }
    public int DaysAfterPlanting { get; set; }
    public NutrientRequirementDto NutrientsToApply { get; set; }
    public List<FertilizerProductDto> Products { get; set; }
    public string ApplicationMethod { get; set; }
    public string Rationale { get; set; }
    public bool IsApplied { get; set; }
    public DateTime? ActualApplicationDate { get; set; }
}
