using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

public class PlanStage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string? StageName { get; set; }

    [Required] public string? Rationale { get; set; }

    public int FertilizationPlanId { get; set; }
    public FertilizationPlan? FertilizationPlan { get; set; }

    public GrowthStage GrowthStage { get; set; }

    /// <summary>
    ///     Коефіцієнт або Зміщення:
    ///     - Для PrePlanting: це дні (e.g., -3)
    ///     - Для Development: це коефіцієнт (e.g., 0.33, тобто 33% від початку фази LDev)
    /// </summary>
    public double TimingFactor { get; set; }

    public int ApplicationMethodId { get; set; }
    public ApplicationMethod? ApplicationMethod { get; set; }

    // Зберігаємо як 0.3 для 30%
    public double NitrogenPercent { get; set; }
    public double PhosphorusPercent { get; set; }
    public double PotassiumPercent { get; set; }
    public double SulfurPercent { get; set; }
    public double CalciumPercent { get; set; }
    public double MagnesiumPercent { get; set; }
}
