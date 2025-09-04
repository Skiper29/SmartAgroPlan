namespace SmartAgroPlan.BLL.DTO.Recommendations;

public class RecommendationResponseDto
{
    public int FieldId { get; set; }
    public string Crop { get; set; } = string.Empty;
    public string GrowthStage { get; set; } = string.Empty;
    public double WaterRecommendationMm { get; set; }
    public double FertilizerRecommendationKgHa { get; set; }
    public string Notes { get; set; } = string.Empty;
}
