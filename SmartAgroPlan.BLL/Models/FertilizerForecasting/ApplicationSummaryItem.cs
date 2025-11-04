namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class ApplicationSummaryItem
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
    public string Stage { get; set; } = null!;
    public NutrientRequirement Nutrients { get; set; } = null!;
}
