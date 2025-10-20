namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

public class FertilizerProductDto
{
    public string Name { get; set; }
    public double NPK_N { get; set; }
    public double NPK_P { get; set; }
    public double NPK_K { get; set; }
    public double QuantityKgPerHa { get; set; }
    public double TotalQuantityKg { get; set; }
    public double? EstimatedCostPerKg { get; set; }
    public double? TotalCost { get; set; }
}
