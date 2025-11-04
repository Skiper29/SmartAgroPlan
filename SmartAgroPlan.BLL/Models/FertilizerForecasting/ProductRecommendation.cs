namespace SmartAgroPlan.BLL.Models.FertilizerForecasting;

/// <summary>
///     Optimized product recommendations for a specific nutrient requirement
/// </summary>
public class ProductRecommendation
{
    public List<RecommendedProduct> Products { get; set; } = new();
    public double TotalCost { get; set; }
    public NutrientRequirement TargetNutrients { get; set; } = null!;
    public NutrientRequirement ActualNutrients { get; set; } = null!;
    public string OptimizationStrategy { get; set; } = null!; // MinCost, MaxEfficiency, Balanced
}

public class RecommendedProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public double QuantityKgPerHa { get; set; }
    public double TotalQuantityKg { get; set; }
    public NutrientRequirement NutrientsProvided { get; set; } = null!;
    public string ApplicationMethod { get; set; } = null!;
    public double? EstimatedCost { get; set; }
}
