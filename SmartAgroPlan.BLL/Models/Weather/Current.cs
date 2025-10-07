namespace SmartAgroPlan.BLL.Models.Weather;

public class Current
{
    public string Time { get; set; }
    public double Temperature_2m { get; set; }
    public double RelativeHumidity_2m { get; set; }
    public double WindSpeed_10m { get; set; }
    public double Surface_Pressure { get; set; }
    public double Soil_Moisture_3_To_9cm { get; set; }
}
