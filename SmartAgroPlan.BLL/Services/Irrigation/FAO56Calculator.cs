using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.BLL.Interfaces.Irrigation;
using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Entities.Irrigation;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Services.Irrigation;

public class FAO56Calculator : IFAO56Calculator
{
    private const double StefanBoltzmannConstant = 4.903e-9; // MJ K^-4 m^-2 day^-1
    private const double PsychrometricConstant = 0.665e-3; // kPa/°C at sea level

    private readonly ICropCoefficientService _cropCoefficientService;

    public FAO56Calculator(ICropCoefficientService cropCoefficientService)
    {
        _cropCoefficientService = cropCoefficientService;
    }

    public double CalculateETc(double et0, double kc)
    {
        return et0 * kc;
    }

    public double CalculateET0(WeatherData weather, double latitude, double elevation)
    {
        // Convert latitude to radians
        var latRad = latitude * Math.PI / 180;

        // Calculate atmospheric pressure if not provided
        var pressure = weather.AtmosphericPressure ?? CalculateAtmosphericPressure(elevation);

        // Psychrometric constant adjusted for elevation
        var gamma = PsychrometricConstant * pressure / 101.3;

        // Mean temperature
        var tMean = (weather.MaxTemperature + weather.MinTemperature) / 2;

        // Saturation vapor pressure
        var esTmax = CalculateSaturationVaporPressure(weather.MaxTemperature);
        var esTmin = CalculateSaturationVaporPressure(weather.MinTemperature);
        var esMean = (esTmax + esTmin) / 2;

        // Actual vapor pressure
        var ea = esMean * weather.RelativeHumidity / 100;

        // Vapor pressure deficit
        var vpd = esMean - ea;

        // Slope of saturation vapor pressure curve
        var delta = CalculateSlopeVaporPressure(tMean);

        // Net radiation
        var rn = CalculateNetRadiation(weather, latRad, elevation);

        // Soil heat flux (assumed negligible for daily calculations)
        double g = 0;

        // Convert wind speed from 10m to 2m height
        var u2 = ConvertWindSpeedTo2M(weather.WindSpeed);
        // FAO-56 Penman-Monteith equation
        var numerator1 = 0.408 * delta * (rn - g);
        var numerator2 = gamma * (900 / (tMean + 273)) * u2 * vpd;
        var denominator = delta + gamma * (1 + 0.34 * u2);

        var et0 = (numerator1 + numerator2) / denominator;

        // Ensure non-negative ET0
        return Math.Max(0, et0);
    }

    public IrrigationRecommendation CalculateIrrigationRequirement(Field field,
        CropCoefficientDefinition cropCoefficientDefinition, WeatherData weather,
        double? currentSoilMoisture = null)
    {
        // Get field centroid for calculations
        var centroid = field.Boundary!.Centroid;

        // Calculate ET0
        var et0 = CalculateET0(weather, centroid.Y, weather.Elevation);

        // Get crop coefficient
        var kc = _cropCoefficientService.GetKc(cropCoefficientDefinition, field.SowingDate ?? DateTime.UtcNow,
            DateTime.UtcNow);

        // Calculate ETc
        var etc = CalculateETc(et0, kc);

        // Effective precipitation (80% of total precipitation)
        var effectivePrecipitation = weather.Precipitation * 0.8;

        // Net irrigation requirement
        var netIrrigationRequirement = etc - effectivePrecipitation;

        // Adjust for current soil moisture if provided
        if (currentSoilMoisture.HasValue && field.Soil != null)
        {
            var fc = GetFieldCapacity(field.Soil.Type);
            var pwp = GetPermanentWiltingPoint(field.Soil.Type);
            var mad = GetManagementAllowableDepletion(field.CurrentCrop!.CropType);

            var threshold = pwp + (fc - pwp) * (1 - mad);

            if (currentSoilMoisture.Value > threshold)
                // Soil moisture is adequate, reduce irrigation
                netIrrigationRequirement *= Math.Max(0, 1 - (currentSoilMoisture.Value - threshold) / (fc - threshold));
        }

        // Application efficiency (typical 85% for drip, 75% for sprinkler)
        var applicationEfficiency = 0.85;
        var grossIrrigation = netIrrigationRequirement / applicationEfficiency;

        var daysSinceSowing = field.SowingDate.HasValue
            ? (DateTime.UtcNow - field.SowingDate.Value).Days
            : 0;

        return new IrrigationRecommendation
        {
            FieldId = field.Id,
            Date = weather.Date,
            ET0 = Math.Round(et0, 2),
            Kc = Math.Round(kc, 2),
            ETc = Math.Round(etc, 2),
            EffectivePrecipitation = Math.Round(effectivePrecipitation, 2),
            NetIrrigationRequirement = Math.Round(Math.Max(0, netIrrigationRequirement), 2),
            GrossIrrigationRequirement = Math.Round(Math.Max(0, grossIrrigation), 2),
            CurrentSoilMoisture = currentSoilMoisture,
            RecommendedAction = GetIrrigationAction(grossIrrigation),
            CropStage = GetCropStage(daysSinceSowing, cropCoefficientDefinition),
            Notes = GenerateRecommendationNotes(grossIrrigation, currentSoilMoisture, weather)
        };
    }

    private double CalculateAtmosphericPressure(double elevation)
    {
        // Atmospheric pressure as function of elevation
        return 101.3 * Math.Pow((293 - 0.0065 * elevation) / 293, 5.26);
    }

    private double CalculateSaturationVaporPressure(double temperature)
    {
        // Tetens equation
        return 0.6108 * Math.Exp(17.27 * temperature / (temperature + 237.3));
    }

    private double CalculateSlopeVaporPressure(double temperature)
    {
        var es = CalculateSaturationVaporPressure(temperature);
        return 4098 * es / Math.Pow(temperature + 237.3, 2);
    }

    private static double ConvertWindSpeedTo2M(double u10)
    {
        // Convert wind speed from 10m to 2m height using logarithmic profile
        return u10 * 4.87 / Math.Log(67.8 * 10 - 5.42);
    }

    private double CalculateNetRadiation(WeatherData weather, double latRad, double elevation)
    {
        // Day of year
        var dayOfYear = weather.Date.DayOfYear;

        // Solar declination
        var declination = 0.409 * Math.Sin(2 * Math.PI * dayOfYear / 365 - 1.39);

        // Sunset hour angle
        var ws = Math.Acos(-Math.Tan(latRad) * Math.Tan(declination));

        // Extraterrestrial radiation
        var dr = 1 + 0.033 * Math.Cos(2 * Math.PI * dayOfYear / 365);
        var ra = 24 * 60 / Math.PI * 0.082 * dr *
                 (ws * Math.Sin(latRad) * Math.Sin(declination) +
                  Math.Cos(latRad) * Math.Cos(declination) * Math.Sin(ws));

        // Clear-sky solar radiation
        var rso = (0.75 + 2e-5 * elevation) * ra;

        // Net shortwave radiation (albedo = 0.23 for reference grass)
        var rns = (1 - 0.23) * weather.SolarRadiation;

        // Net longwave radiation
        var relativeShortwave = Math.Min(weather.SolarRadiation / rso, 1.0);
        var tKelvinMax = weather.MaxTemperature + 273.16;
        var tKelvinMin = weather.MinTemperature + 273.16;
        var ea = CalculateSaturationVaporPressure(weather.Temperature) * weather.RelativeHumidity / 100;

        var rnl = StefanBoltzmannConstant *
                  ((Math.Pow(tKelvinMax, 4) + Math.Pow(tKelvinMin, 4)) / 2) *
                  (0.34 - 0.14 * Math.Sqrt(ea)) *
                  (1.35 * relativeShortwave - 0.35);

        // Net radiation
        return rns - rnl;
    }

    private double GetFieldCapacity(SoilType soilType)
    {
        return soilType switch
        {
            _ => 25
        };
    }

    private double GetPermanentWiltingPoint(SoilType soilType)
    {
        return soilType switch
        {
            _ => 12
        };
    }

    private double GetManagementAllowableDepletion(CropType cropType)
    {
        // MAD varies by crop sensitivity to water stress
        return cropType switch
        {
            _ => 0.50
        };
    }

    private string GetCropStage(int daysSinceSowing, CropCoefficientDefinition cropCoefficientDefinition)
    {
        var totalGrowthDays = cropCoefficientDefinition.LIni + cropCoefficientDefinition.LDev +
                              cropCoefficientDefinition.LMid + cropCoefficientDefinition.LLate;

        if (daysSinceSowing <= cropCoefficientDefinition.LIni) return "Початкова стадія";
        if (daysSinceSowing <= cropCoefficientDefinition.LIni + cropCoefficientDefinition.LDev) return "Розвиток";
        if (daysSinceSowing <= cropCoefficientDefinition.LIni + cropCoefficientDefinition.LDev +
            cropCoefficientDefinition.LMid) return "Середній стадія";
        return daysSinceSowing <= totalGrowthDays ? "Пізня стадія" : "Збір врожаю";
    }

    private string GetIrrigationAction(double grossIrrigation)
    {
        return grossIrrigation switch
        {
            <= 0 => "Не потрібно зрошення",
            <= 5 => "Легке зрошення рекомендовано",
            <= 15 => "Середнє зрошення рекомендовано",
            <= 25 => "Інтенсивне зрошення рекомендовано",
            _ => "Дуже інтенсивне зрошення рекомендовано"
        };
    }

    private string GenerateRecommendationNotes(double grossIrrigation, double? soilMoisture, WeatherData weather)
    {
        var notes = new List<string>();

        if (grossIrrigation <= 0)
            notes.Add("Достатня вологість ґрунту та/або нещодавні опади.");
        else if (grossIrrigation > 20)
            notes.Add("Виявлено значний дефіцит води. Розгляньте можливість термінового зрошення.");

        if (soilMoisture is < 20)
            notes.Add("Низький рівень вологості ґрунту. Ретельно контролюйте.");

        if (weather.WindSpeed > 5) notes.Add("Сильний вітер. Уникайте зрошення за допомогою розпилювачів.");

        if (weather.Temperature > 35) notes.Add("Висока температура. Розгляньте можливість вечірнього поливу.");

        return string.Join(" ", notes);
    }
}
