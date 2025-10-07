namespace SmartAgroPlan.BLL.Models.Weather;

public class Hourly
{
    public List<string> Time { get; set; }
    public List<double> Temperature_2m { get; set; }
    public List<double> RelativeHumidity_2m { get; set; }
    public List<double> WindSpeed_10m { get; set; }
    public List<double> Surface_Pressure { get; set; }
    public List<double> Soil_Moisture_3_To_9cm { get; set; }
}
