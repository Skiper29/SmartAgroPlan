namespace SmartAgroPlan.BLL.DTO.Irrigation.Weather;

public class WeatherConditionsDto
{
    public double Temperature { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
    public double RelativeHumidity { get; set; }
    public double WindSpeed { get; set; }
    public double SolarRadiation { get; set; }
    public double Precipitation { get; set; }
}
