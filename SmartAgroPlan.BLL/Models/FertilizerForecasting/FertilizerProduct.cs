namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

public class FertilizerProduct
{
    public string Name { get; set; } = null!;
    public double NPK_N { get; set; }
    public double NPK_P { get; set; }
    public double NPK_K { get; set; }
    public double QuantityKgPerHa { get; set; }
    public double TotalQuantityKg { get; set; }
}
