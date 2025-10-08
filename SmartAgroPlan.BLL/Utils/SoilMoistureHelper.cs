using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Utils;

public static class SoilMoistureHelper
{
    public static double GetSoilMoisture(ICollection<FieldCondition>? fieldConditions, WeatherData weatherData)
    {
        var soilMoisture = weatherData.SoilMoisture;
        FieldCondition? latestCondition = null;
        if (fieldConditions != null && fieldConditions.Count != 0)
            latestCondition = fieldConditions
                .Where(c => c.SoilMoisture.HasValue)
                .OrderByDescending(c => c.RecordedAt)
                .FirstOrDefault();

        if (latestCondition != null && latestCondition.RecordedAt.Date > DateTime.UtcNow.Date.AddDays(-7))
            soilMoisture = latestCondition.SoilMoisture!.Value;

        return soilMoisture;
    }

    /// <summary>
    ///     Updates the soil moisture based on ET0, precipitation, and net irrigation.
    ///     Soil moisture is in m3/m3 (0 to 1).
    ///     ET0, precipitation, and net irrigation are in mm.
    /// </summary>
    public static double UpdateSoilMoisture(double currentSoilMoisture, double et0, double precipitation,
        double netIrrigation)
    {
        // Convert soil moisture from m3/m3 to mm (assuming a root zone depth of 300mm)
        var rootZoneDepth = 300; // mm
        var soilMoistureMm = currentSoilMoisture * rootZoneDepth;

        // Update soil moisture
        soilMoistureMm -= et0; // Evapotranspiration reduces soil moisture
        soilMoistureMm += precipitation; // Precipitation increases soil moisture
        soilMoistureMm += netIrrigation; // Irrigation increases soil moisture

        // Ensure soil moisture is within realistic bounds
        if (soilMoistureMm < 0) soilMoistureMm = 0;
        if (soilMoistureMm > rootZoneDepth) soilMoistureMm = rootZoneDepth;

        // Convert back to m3/m3
        return soilMoistureMm / rootZoneDepth;
    }
}
