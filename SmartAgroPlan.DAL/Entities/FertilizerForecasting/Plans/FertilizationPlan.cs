using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

public class FertilizationPlan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [StringLength(100)] public string? Name { get; set; }

    [StringLength(500)] public string? Description { get; set; }

    public CropType CropType { get; set; }

    public ICollection<PlanStage> Stages { get; set; } = new List<PlanStage>();
}
