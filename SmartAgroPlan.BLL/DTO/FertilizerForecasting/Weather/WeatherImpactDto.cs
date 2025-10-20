namespace SmartAgroPlan.BLL.DTO.FertilizerForecasting.Weather;

public class WeatherImpactDto
{
    public double ExpectedRainfall7Days { get; set; }
    public double AvgTemperature7Days { get; set; }
    public string Impact { get; set; }
    public List<string> Adjustments { get; set; }
}
