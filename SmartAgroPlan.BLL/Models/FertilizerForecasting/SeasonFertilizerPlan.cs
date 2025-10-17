namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class SeasonFertilizerPlan
{
    public int FieldId { get; set; }
    public string CropName { get; set; } = null!;
    public DateTime PlanGeneratedDate { get; set; }
    public NutrientRequirement TotalSeasonRequirement { get; set; } = null!;
    public NutrientRequirement SoilSupply { get; set; } = null!;
    public NutrientRequirement RequiredFromFertilizer { get; set; } = null!;
    public List<FertilizerApplication> Applications { get; set; } = null!;
    public double FieldAreaHa { get; set; }
    public double ExpectedYield { get; set; }
    public string Notes { get; set; } = null!;
}
