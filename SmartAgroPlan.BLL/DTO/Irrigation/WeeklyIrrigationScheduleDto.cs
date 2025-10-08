namespace SmartAgroPlan.BLL.DTO.Irrigation;

public class WeeklyIrrigationScheduleDto
{
    public int FieldId { get; set; }
    public string FieldName { get; set; }
    public string CropType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<DailyIrrigationDto> DailySchedule { get; set; }
    public double TotalWaterRequirement { get; set; }
    public double TotalExpectedPrecipitation { get; set; }
    public int IrrigationDays { get; set; }
    public string Recommendations { get; set; }
}
