namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

public class NutrientDeficitAnalysisDto
{
    public int FieldId { get; set; }
    public DateTime AnalysisDate { get; set; }
    public string CurrentStage { get; set; }

    public NutrientDeficitDto NitrogenStatus { get; set; }
    public NutrientDeficitDto PhosphorusStatus { get; set; }
    public NutrientDeficitDto PotassiumStatus { get; set; }

    public string OverallStatus { get; set; }
    public List<string> Recommendations { get; set; }
    public bool UrgentActionNeeded { get; set; }
}
