namespace SmartAgroPlan.BLL.Models.FertilizerForecasting.Nutrients;

public class NutrientDeficitAnalysis
{
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public List<NutrientDeficit> Deficits { get; set; } = new();
    public DateTime AnalysisDate { get; set; }
    public string OverallStatus { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
}
