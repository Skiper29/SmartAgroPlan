using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

public class NutrientApplicationSummaryDto
{
    public int FieldId { get; set; }
    public string? FieldName { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public NutrientRequirementDto TotalApplied { get; set; } = null!;
    public NutrientRequirementDto PlannedToApply { get; set; } = null!;
    public int CompletedApplications { get; set; }
    public int PendingApplications { get; set; }
    public List<ApplicationSummaryItemDto> Applications { get; set; } = new();
}
