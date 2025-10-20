namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Products;

public class FertilizerCostEstimateDto
{
    public double TotalEstimatedCost { get; set; }
    public double CostPerHectare { get; set; }
    public Dictionary<string, double> CostByProduct { get; set; }
}
