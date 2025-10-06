namespace SmartAgroPlan.BLL.Models.Weather;

public class OpenMeteoSimpleResponse : OpenMeteoBasicResponse
{
    public Hourly Hourly { get; set; }
}
