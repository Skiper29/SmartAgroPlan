using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting;

public class ApplicationMethod
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] public string? Name { get; set; }

    [Required] public string? Description { get; set; }

    public ICollection<PlanStage> PlanStages { get; set; } = new List<PlanStage>();
}
