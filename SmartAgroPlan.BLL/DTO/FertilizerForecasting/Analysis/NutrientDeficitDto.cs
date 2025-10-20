namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

public class NutrientDeficitDto
{
    public double CurrentLevel { get; set; }
    public double RequiredLevel { get; set; }
    public double Deficit { get; set; }
    public double DeficitPercentage { get; set; }
    public string Status { get; set; } // "Sufficient", "Marginal", "Deficient", "Severely Deficient"
}
