namespace SmartAgroPlan.BLL.Models.Weather;

public class Daily
{
    public List<string> Time { get; set; }
    public List<double> Temperature_2m_max { get; set; }
    public List<double> Temperature_2m_min { get; set; }
    public List<double> Precipitation_sum { get; set; }
    public List<double> Shortwave_radiation_sum { get; set; }
}
