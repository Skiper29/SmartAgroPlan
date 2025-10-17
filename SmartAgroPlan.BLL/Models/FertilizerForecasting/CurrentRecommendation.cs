namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class CurrentRecommendation
{
    public DateTime Date { get; set; }
    public string CurrentStage { get; set; } = null!;
    public int DaysAfterPlanting { get; set; }
    public bool ShouldApplyNow { get; set; }
    public NutrientRequirement RecommendedNutrients { get; set; } = null!;
    public List<FertilizerProduct> Products { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string Reasoning { get; set; } = null!;
    public List<string> Warnings { get; set; } = null!;
}
