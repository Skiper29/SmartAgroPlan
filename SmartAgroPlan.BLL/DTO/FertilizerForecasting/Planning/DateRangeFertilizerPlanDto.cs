using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Application;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Nutrients;

namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

public class DateRangeFertilizerPlanDto
{
    public int FieldId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<FertilizerApplicationDto> ApplicationsInRange { get; set; }
    public NutrientRequirementDto TotalNutrientsInRange { get; set; }
    public double EstimatedCost { get; set; }
}
