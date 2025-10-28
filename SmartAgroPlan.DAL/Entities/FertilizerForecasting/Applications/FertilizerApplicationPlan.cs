using System.ComponentModel.DataAnnotations;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Records;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Applications;

/// <summary>
///     Stores planned fertilizer applications for a field
/// </summary>
public class FertilizerApplicationPlan
{
    [Key] public int Id { get; set; }

    [Required] public int FieldId { get; set; }

    [Required] public int FertilizationPlanId { get; set; }

    [Required] public int PlanStageId { get; set; }

    [Required] public DateTime CreatedDate { get; set; }

    [Required] public DateTime PlannedApplicationDate { get; set; }

    public int DaysAfterPlanting { get; set; }

    // Planned nutrients (kg/ha)
    public double PlannedNitrogen { get; set; }
    public double PlannedPhosphorus { get; set; }
    public double PlannedPotassium { get; set; }
    public double PlannedSulfur { get; set; }
    public double PlannedCalcium { get; set; }
    public double PlannedMagnesium { get; set; }
    public double PlannedBoron { get; set; }
    public double PlannedZinc { get; set; }
    public double PlannedManganese { get; set; }
    public double PlannedCopper { get; set; }
    public double PlannedIron { get; set; }
    public double PlannedMolybdenum { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime? ActualApplicationDate { get; set; }

    [StringLength(500)] public string? Notes { get; set; }

    // Navigation properties
    public Field? Field { get; set; }
    public FertilizationPlan? FertilizationPlan { get; set; }
    public PlanStage? PlanStage { get; set; }
    public ICollection<FertilizerApplicationProduct> Products { get; set; } = new List<FertilizerApplicationProduct>();
    public FertilizerApplicationRecord? CompletionRecord { get; set; }
}
