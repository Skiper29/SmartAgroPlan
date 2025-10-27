using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartAgroPlan.DAL.Entities.FertilizerForecasting.Plans;

namespace SmartAgroPlan.DAL.Entities.FertilizerForecasting;

public class ApplicationMethod
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [StringLength(100)] public string? Name { get; set; }

    [StringLength(500)] public string? Description { get; set; }

    public ICollection<PlanStage> PlanStages { get; set; } = new List<PlanStage>();
}
