using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

public class ApplicationSummaryItemDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
    public string Stage { get; set; } = null!;
    public NutrientRequirementDto Nutrients { get; set; } = null!;
}
