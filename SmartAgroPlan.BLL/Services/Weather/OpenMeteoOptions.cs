namespace SmartAgroPlan.BLL.Services.Weather;

public class OpenMeteoOptions
{
    public string BaseUrl { get; set; } = "https://api.open-meteo.com/v1";

    public string DailyParams { get; set; } =
        "temperature_2m_max,temperature_2m_min,precipitation_sum,shortwave_radiation_sum";

    public string HourlyParams { get; set; } =
        "temperature_2m,relativehumidity_2m,windspeed_10m,surface_pressure,soil_moisture_3_to_9cm";
}
