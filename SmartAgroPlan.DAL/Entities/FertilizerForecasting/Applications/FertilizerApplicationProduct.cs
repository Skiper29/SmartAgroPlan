using System.ComponentModel.DataAnnotations;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;

/// <summary>
///     Stores specific fertilizer products planned for an application
/// </summary>
public class FertilizerApplicationProduct
{
    [Key] public int Id { get; set; }

    [Required] public int ApplicationPlanId { get; set; }

    [Required] public int FertilizerProductId { get; set; }

    public double QuantityKgPerHa { get; set; }
    public double TotalQuantityKg { get; set; }

    // Navigation properties
    public FertilizerApplicationPlan? ApplicationPlan { get; set; }
    public FertilizerProduct? FertilizerProduct { get; set; }
}
