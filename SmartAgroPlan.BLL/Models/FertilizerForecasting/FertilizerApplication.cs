namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class FertilizerApplication
{
    public DateTime RecommendedDate { get; set; }
    public string CropStage { get; set; } = null!;
    public int DaysAfterPlanting { get; set; }
    public NutrientRequirement NutrientsToApply { get; set; } = null!;
    public List<FertilizerProduct> Products { get; set; } = null!;
    public string ApplicationMethod { get; set; } = null!;
    public string Rationale { get; set; } = null!;
}
