namespace SmartAgroPlan.BLL.DTO.Irrigation;

public class IrrigationForecastDto
{
    public DateTime Date { get; set; }
    public double ET0 { get; set; }
    public double ETc { get; set; }
    public double ExpectedPrecipitation { get; set; }
    public double NetIrrigationRequirement { get; set; }
    public double GrossIrrigationRequirement { get; set; }
}
