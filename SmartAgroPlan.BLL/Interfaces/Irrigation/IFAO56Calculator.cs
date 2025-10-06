using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Entities.Irrigation;

namespace SmartAgroPlan.BLL.Interfaces.Irrigation;

public interface IFAO56Calculator
{
    double CalculateET0(WeatherData weather, double latitude, double elevation);
    double CalculateETc(double et0, double kc);

    IrrigationRecommendation CalculateIrrigationRequirement(
        Field field,
        WeatherData weather,
        double? currentSoilMoisture = null);
}
