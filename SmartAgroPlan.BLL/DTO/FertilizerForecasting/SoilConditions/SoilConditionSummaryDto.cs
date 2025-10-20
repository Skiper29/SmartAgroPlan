namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.SoilConditions;

public class SoilConditionSummaryDto
{
    public double? SoilMoisture { get; set; }
    public double? SoilPh { get; set; }
    public double? Nitrogen { get; set; }
    public double? Phosphorus { get; set; }
    public double? Potassium { get; set; }
    public DateTime LastRecordedAt { get; set; }
}
