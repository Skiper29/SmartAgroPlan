namespace SmartAgroPlan.BLL.DTO.Irrigation.Schedule;

public class DailyIrrigationDto
{
    public DateTime Date { get; set; }
    public string DayOfWeek { get; set; }
    public double ET0 { get; set; }
    public double ETc { get; set; }
    public double Precipitation { get; set; }
    public double NetIrrigationRequired { get; set; }
    public double GrossIrrigationRequired { get; set; }
    public double SoilMoisture { get; set; }
    public bool ShouldIrrigate { get; set; }
    public string RecommendedTime { get; set; }
    public string WeatherSummary { get; set; }
}
