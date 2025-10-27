using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

public class FertilizationPlan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string? Name { get; set; }

    [Required] public string? Description { get; set; }

    public CropType CropType { get; set; }

    public ICollection<PlanStage> Stages { get; set; } = new List<PlanStage>();
}
