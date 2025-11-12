using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;

public class FertilizerApplicationDto
{
    public int? Id { get; set; }
    public DateTime RecommendedDate { get; set; }
    public string CropStage { get; set; } = null!;
    public int DaysAfterPlanting { get; set; }
    public NutrientRequirementDto NutrientsToApply { get; set; } = null!;
    public List<FertilizerProductDto> Products { get; set; } = new();
    public string ApplicationMethod { get; set; } = null!;
    public string Rationale { get; set; } = null!;
    public string? WeatherConsiderations { get; set; }
    public string? Warnings { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? ActualApplicationDate { get; set; }
}
