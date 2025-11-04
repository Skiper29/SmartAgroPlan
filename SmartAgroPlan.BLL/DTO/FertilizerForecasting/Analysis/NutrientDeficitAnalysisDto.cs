namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

public class NutrientDeficitAnalysisDto
{
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
    public DateTime AnalysisDate { get; set; }
    public List<NutrientDeficitDto> Deficits { get; set; } = new();
    public string OverallStatus { get; set; } = null!;
    public List<string> Recommendations { get; set; } = null!;
}
