namespace SmartAgroPlan.BLL.Models.Weather;

public class WeatherData
{
    public double Temperature { get; set; } // °C
    public double MinTemperature { get; set; } // °C
    public double MaxTemperature { get; set; } // °C
    public double RelativeHumidity { get; set; } // %
    public double WindSpeed { get; set; } // m/s at 2m height
    public double SolarRadiation { get; set; } // MJ/m²/day
    public double Precipitation { get; set; } // mm
    public DateTime Date { get; set; }
    public double? AtmosphericPressure { get; set; } // kPa
}
