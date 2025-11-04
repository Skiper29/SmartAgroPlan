namespace SmartAgroPlan.BLL.Models.FertilizerForecasting.Nutrients;

public class NutrientDeficit
{
    public string NutrientName { get; set; } = null!;
    public double DeficitAmount { get; set; } // in kg/ha
    public string Urgency { get; set; } = null!; // Low, Medium, High
    public string Symptoms { get; set; } = null!;
}
