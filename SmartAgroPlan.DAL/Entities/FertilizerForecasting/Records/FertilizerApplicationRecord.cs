using System.ComponentModel.DataAnnotations;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;

/// <summary>
///     Records actual fertilizer applications performed
/// </summary>
public class FertilizerApplicationRecord
{
    [Key] public int Id { get; set; }

    [Required] public int FieldId { get; set; }

    public int? ApplicationPlanId { get; set; }

    [Required] public DateTime ApplicationDate { get; set; }

    [Required] public DateTime RecordedDate { get; set; }

    // Actual nutrients applied (kg/ha)
    public double AppliedNitrogen { get; set; }
    public double AppliedPhosphorus { get; set; }
    public double AppliedPotassium { get; set; }
    public double AppliedSulfur { get; set; }
    public double AppliedCalcium { get; set; }
    public double AppliedMagnesium { get; set; }
    public double AppliedBoron { get; set; }
    public double AppliedZinc { get; set; }
    public double AppliedManganese { get; set; }
    public double AppliedCopper { get; set; }
    public double AppliedIron { get; set; }
    public double AppliedMolybdenum { get; set; }

    [Required] public int ApplicationMethodId { get; set; }

    [StringLength(1000)] public string? Notes { get; set; }

    // Weather conditions during application
    public double? TemperatureC { get; set; }
    public double? WindSpeedKmh { get; set; }
    public double? Humidity { get; set; }

    // Navigation properties
    public Field? Field { get; set; }
    public FertilizerApplicationPlan? ApplicationPlan { get; set; }
    public ApplicationMethod? ApplicationMethod { get; set; }

    public ICollection<FertilizerApplicationRecordProduct> ProductsUsed { get; set; } =
        new List<FertilizerApplicationRecordProduct>();
}
