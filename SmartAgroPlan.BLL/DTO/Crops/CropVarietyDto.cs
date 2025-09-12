using SmartAgroPlan.DAL.Entities.Calendar;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.DTO.Crops;

public class CropVarietyDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public CropType CropType { get; set; }
    public double WaterRequirement { get; set; }
    public double FertilizerRequirement { get; set; }
    public int GrowingDuration { get; set; }
    public DayMonth SowingStart { get; set; }
    public DayMonth SowingEnd { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
    public double HarvestYield { get; set; }
    public string? AdditionalNotes { get; set; }
    public SoilType OptimalSoil { get; set; }
}
