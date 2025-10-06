namespace SmartAgroPlan.BLL.Models.Weather;

public class OpenMeteoBasicResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
    public Daily Daily { get; set; }
}
