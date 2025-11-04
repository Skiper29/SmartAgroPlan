using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class CurrentRecommendation
{
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
    public DateTime Date { get; set; }
    public string CurrentStage { get; set; } = null!;
    public int DaysAfterPlanting { get; set; }
    public int DaysToHarvest { get; set; }
    public bool ShouldApplyNow { get; set; }
    public NutrientRequirement RecommendedNutrients { get; set; } = null!;
    public List<FertilizerProductDto> Products { get; set; } = new();
    public string ApplicationMethod { get; set; } = null!;
    public string Priority { get; set; } = null!; // Low, Medium, High, Critical
    public string Reasoning { get; set; } = null!;
    public List<string> Warnings { get; set; } = new();
    public string? WeatherConsiderations { get; set; }
    public DateTime? NextRecommendedDate { get; set; }
}
