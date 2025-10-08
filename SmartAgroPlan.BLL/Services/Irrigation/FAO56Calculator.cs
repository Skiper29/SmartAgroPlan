using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.BLL.Interfaces.Irrigation;
using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Entities.Irrigation;
using SmartAgroPlan.DAL.Enums;

namespace SmartAgroPlan.BLL.Services.Irrigation;

/// <summary>
///     FAO-56 Penman-Monteith evapotranspiration and irrigation calculator.
///     Based on FAO Irrigation and Drainage Paper 56.
/// </summary>
public class FAO56Calculator : IFAO56Calculator
{
    // Stefan-Boltzmann constant for longwave radiation (MJ K^-4 m^-2 day^-1)
    // Used in net longwave radiation calculation (Equation 39)
    private const double StefanBoltzmannConstant = 4.903e-9;

    // Psychrometric constant coefficient at sea level (kPa/°C)
    // γ = 0.665 × 10^-3 × P/101.3, where P is atmospheric pressure
    private const double PsychrometricConstantCoefficient = 0.665e-3;

    // Standard atmospheric pressure at sea level (kPa)
    private const double StandardAtmosphericPressure = 101.3;

    // Latent heat of vaporization (MJ/kg) at 20°C
    // Energy required to change water from liquid to vapor
    private const double LatentHeatOfVaporization = 2.45;

    // Specific heat at constant pressure (MJ kg^-1 °C^-1)
    // Amount of energy to increase air temperature by 1°C
    private const double SpecificHeatAtConstantPressure = 1.013e-3;

    // Ratio of molecular weight of water vapor to dry air
    private const double MolecularWeightRatio = 0.622;

    // Solar constant (MJ m^-2 min^-1)
    // Radiation at top of atmosphere perpendicular to sun's rays
    private const double SolarConstant = 0.0820;

    // Conversion factor from radiation (MJ m^-2 day^-1) to equivalent evaporation (mm/day)
    // Equals 1/λ = 1/2.45 ≈ 0.408
    private const double RadiationToEvaporationFactor = 0.408;

    // Albedo (reflection coefficient) for reference grass crop
    // Fraction of solar radiation reflected from surface
    private const double ReferenceAlbedo = 0.23;

    // Numerator constant for FAO-56 PM equation (K mm s^3 Mg^-1 day^-1)
    // For grass reference at 2m height and daily time step
    private const double NumeratorConstant = 900;

    // Denominator constant for FAO-56 PM equation (s m^-1)
    // For grass reference at 2m height
    private const double DenominatorConstant = 0.34;

    // Default root depth for irrigation calculations (meters)
    // Typical effective root zone depth
    private const double DefaultRootDepth = 0.3;

    // Effective precipitation coefficient
    // Fraction of precipitation that infiltrates and is available to plants
    private const double EffectivePrecipitationCoefficient = 0.8;

    // Application efficiency for drip irrigation (fraction)
    private const double DripIrrigationEfficiency = 0.85;

    // Application efficiency for sprinkler irrigation (fraction)
    private const double SprinklerIrrigationEfficiency = 0.75;

    // Elevation adjustment coefficient for clear-sky radiation (per meter)
    // Rso = (0.75 + 2×10^-5 × elevation) × Ra
    private const double ElevationAdjustmentCoefficient = 2e-5;

    // Base clear-sky coefficient (dimensionless)
    private const double BaseClearSkyCoefficient = 0.75;

    // Vapor pressure adjustment coefficient for net longwave radiation
    private const double VaporPressureCoefficient1 = 0.34;
    private const double VaporPressureCoefficient2 = 0.14;

    // Cloudiness adjustment coefficients for net longwave radiation
    private const double CloudinessCoefficient1 = 1.35;
    private const double CloudinessCoefficient2 = 0.35;

    // Temperature lapse rate (°C per meter)
    // Rate at which temperature decreases with altitude
    private const double TemperatureLapseRate = 0.0065;

    // Standard temperature at sea level (Kelvin)
    private const double StandardTemperature = 293.0;

    // Atmospheric pressure exponent
    private const double AtmosphericPressureExponent = 5.26;

    // Conversion from Celsius to Kelvin
    private const double CelsiusToKelvinOffset = 273.16;

    // Solar declination coefficient (radians)
    private const double SolarDeclinationAmplitude = 0.409;

    // Solar declination phase offset (radians)
    private const double SolarDeclinationPhaseOffset = 1.39;

    // High wind speed threshold (m/s) for irrigation recommendations
    private const double HighWindSpeedThreshold = 5.0;

    // High temperature threshold (°C) for irrigation timing recommendations
    private const double HighTemperatureThreshold = 35.0;

    // Minimum soil moisture threshold multiplier
    private const double MinimumSoilMoistureEpsilon = 1e-6;

    // Minimum wind speed for ET0 calculation (m/s)
    // Accounts for boundary layer instability when air is calm
    private const double MinimumWindSpeed = 0.5;

    // Base saturation vapor pressure at 0°C (kPa)
    private const double SaturationVaporPressureBase = 0.6108;

    // Temperature coefficient in Tetens equation
    private const double TetensCoefficient1 = 17.27;

    // Temperature offset in Tetens equation (°C)
    private const double TetensCoefficient2 = 237.3;

    private readonly ICropCoefficientService _cropCoefficientService;
    private readonly ISoilWaterService _soilWaterService;

    public FAO56Calculator(ICropCoefficientService cropCoefficientService, ISoilWaterService soilWaterService)
    {
        _cropCoefficientService = cropCoefficientService;
        _soilWaterService = soilWaterService;
    }

    /// <summary>
    ///     Calculates crop evapotranspiration (ETc) from reference ET0 and crop coefficient.
    ///     ETc = ET0 × Kc
    /// </summary>
    public double CalculateETc(double et0, double kc)
    {
        return et0 * kc;
    }

    /// <summary>
    ///     Calculates reference evapotranspiration (ET0) using FAO-56 Penman-Monteith equation.
    /// </summary>
    public double CalculateET0(WeatherData weather, double latitude, double elevation)
    {
        // Convert latitude to radians
        var latRad = latitude * Math.PI / 180;

        // Calculate atmospheric pressure if not provided
        var pressure = weather.AtmosphericPressure ?? CalculateAtmosphericPressure(elevation);

        // Psychrometric constant adjusted for elevation
        var gamma = PsychrometricConstantCoefficient * pressure / StandardAtmosphericPressure;

        // Mean temperature
        var tMean = (weather.MaxTemperature + weather.MinTemperature) / 2.0;

        // Saturation vapor pressure
        var esTmax = CalculateSaturationVaporPressure(weather.MaxTemperature);
        var esTmin = CalculateSaturationVaporPressure(weather.MinTemperature);
        var esMean = (esTmax + esTmin) / 2.0;

        // Actual vapor pressure
        var ea = esMean * weather.RelativeHumidity / 100.0;

        // Vapor pressure deficit
        var vpd = esMean - ea;

        // Slope of saturation vapor pressure curve
        var delta = CalculateSlopeVaporPressure(tMean);

        // Net radiation
        var rn = CalculateNetRadiation(weather, latRad, elevation);

        // Soil heat flux (assumed negligible for daily calculations)
        const double g = 0.0;

        // Ensure minimum wind speed to account for boundary layer effects
        var windSpeed = Math.Max(weather.WindSpeed, MinimumWindSpeed);

        // FAO-56 Penman-Monteith equation
        var numerator1 = RadiationToEvaporationFactor * delta * (rn - g);
        var numerator2 = gamma * (NumeratorConstant / (tMean + CelsiusToKelvinOffset)) * windSpeed * vpd;
        var denominator = delta + gamma * (1.0 + DenominatorConstant * windSpeed);

        var et0 = (numerator1 + numerator2) / denominator;

        // Ensure non-negative ET0
        return Math.Max(0.0, et0);
    }

    /// <summary>
    ///     Calculates irrigation recommendation based on FAO-56 water balance approach.
    /// </summary>
    public IrrigationRecommendation CalculateIrrigationRequirement(
        Field field,
        CropCoefficientDefinition cropCoefficientDefinition,
        WeatherData weather,
        double? currentSoilMoisture = null)
    {
        // Get field centroid for calculations
        var centroid = field.Boundary!.Centroid;

        // Calculate ET0
        var et0 = CalculateET0(weather, centroid.Y, weather.Elevation);

        // Get crop coefficient
        var kc = _cropCoefficientService.GetKc(
            cropCoefficientDefinition,
            field.SowingDate ?? DateTime.UtcNow,
            DateTime.UtcNow);

        // Calculate ETc
        var etc = CalculateETc(et0, kc);

        // Effective precipitation (typically 80% of total)
        var effectivePrecipitation = weather.Precipitation * EffectivePrecipitationCoefficient;

        // Net irrigation requirement (before soil moisture adjustment)
        var netIrrigationRequirement = etc - effectivePrecipitation;

        // Adjust for current soil moisture if provided
        if (currentSoilMoisture.HasValue && field.Soil != null)
            netIrrigationRequirement = AdjustForSoilMoisture(
                netIrrigationRequirement,
                currentSoilMoisture.Value,
                field.Soil.Type,
                field.CurrentCrop!.CropType);

        // Application efficiency (drip irrigation default)
        var grossIrrigation = netIrrigationRequirement / DripIrrigationEfficiency;

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
            Notes = GenerateRecommendationNotes(
                grossIrrigation,
                currentSoilMoisture,
                weather,
                field.Soil!.Type,
                field.CurrentCrop!.CropType)
        };
    }

    /// <summary>
    ///     Calculates atmospheric pressure as function of elevation (Equation 7).
    ///     P = 101.3 × [(293 - 0.0065 × z) / 293]^5.26
    /// </summary>
    private double CalculateAtmosphericPressure(double elevation)
    {
        return StandardAtmosphericPressure *
               Math.Pow((StandardTemperature - TemperatureLapseRate * elevation) / StandardTemperature,
                   AtmosphericPressureExponent);
    }

    /// <summary>
    ///     Calculates saturation vapor pressure using Tetens equation (Equation 11).
    ///     e°(T) = 0.6108 × exp[17.27 × T / (T + 237.3)]
    /// </summary>
    private double CalculateSaturationVaporPressure(double temperature)
    {
        return SaturationVaporPressureBase *
               Math.Exp(TetensCoefficient1 * temperature / (temperature + TetensCoefficient2));
    }

    /// <summary>
    ///     Calculates slope of saturation vapor pressure curve (Equation 13).
    ///     Δ = 4098 × [0.6108 × exp(17.27T/(T+237.3))] / (T + 237.3)²
    /// </summary>
    private double CalculateSlopeVaporPressure(double temperature)
    {
        var es = CalculateSaturationVaporPressure(temperature);
        return 4098.0 * es / Math.Pow(temperature + TetensCoefficient2, 2);
    }

    /// <summary>
    ///     Calculates net radiation (Rn = Rns - Rnl) (Equations 38-40).
    /// </summary>
    private double CalculateNetRadiation(WeatherData weather, double latRad, double elevation)
    {
        // Day of year
        var dayOfYear = weather.Date.DayOfYear;

        // Solar declination (Equation 24)
        var declination = SolarDeclinationAmplitude *
                          Math.Sin(2.0 * Math.PI * dayOfYear / 365.0 - SolarDeclinationPhaseOffset);

        // Sunset hour angle (Equation 25)
        var ws = Math.Acos(-Math.Tan(latRad) * Math.Tan(declination));

        // Extraterrestrial radiation (Equation 21)
        var dr = 1.0 + 0.033 * Math.Cos(2.0 * Math.PI * dayOfYear / 365.0);
        var ra = 24.0 * 60.0 / Math.PI * SolarConstant * dr *
                 (ws * Math.Sin(latRad) * Math.Sin(declination) +
                  Math.Cos(latRad) * Math.Cos(declination) * Math.Sin(ws));

        // Clear-sky solar radiation (Equation 37)
        var rso = (BaseClearSkyCoefficient + ElevationAdjustmentCoefficient * elevation) * ra;

        // Net shortwave radiation (Equation 38)
        var rns = (1.0 - ReferenceAlbedo) * weather.SolarRadiation;

        // Net longwave radiation (Equation 39)
        var relativeShortwave = Math.Min(weather.SolarRadiation / rso, 1.0);
        var tKelvinMax = weather.MaxTemperature + CelsiusToKelvinOffset;
        var tKelvinMin = weather.MinTemperature + CelsiusToKelvinOffset;
        var ea = CalculateSaturationVaporPressure(weather.Temperature) *
            weather.RelativeHumidity / 100.0;

        var rnl = StefanBoltzmannConstant *
                  ((Math.Pow(tKelvinMax, 4) + Math.Pow(tKelvinMin, 4)) / 2.0) *
                  (VaporPressureCoefficient1 - VaporPressureCoefficient2 * Math.Sqrt(ea)) *
                  (CloudinessCoefficient1 * relativeShortwave - CloudinessCoefficient2);

        // Net radiation
        return rns - rnl;
    }

    /// <summary>
    ///     Adjusts irrigation requirement based on current soil moisture deficit.
    ///     Uses FAO-56 water balance approach with management allowed depletion (MAD).
    /// </summary>
    private double AdjustForSoilMoisture(
        double baseRequirement,
        double currentVWC,
        SoilType soilType,
        CropType cropType)
    {
        var sw = _soilWaterService.GetSoilParams(soilType, cropType);

        var fc = sw.FieldCapacity;
        var pwp = sw.WiltingPoint;
        var mad = sw.AllowableDepletionFraction;

        // Total available water (mm) in root zone
        var taw = (fc - pwp) * DefaultRootDepth * 1000.0;

        // Current soil moisture deficit (mm)
        var currentDeficit = (fc - currentVWC) * DefaultRootDepth * 1000.0;

        // Readily available water threshold (mm)
        var raw = mad * taw;

        if (currentDeficit > raw)
            // Deficit exceeds allowable - need to refill to field capacity
            return currentDeficit;

        // Scale irrigation based on current deficit relative to allowable
        var stressReduction = Math.Clamp(1.0 - currentDeficit / raw, 0.0, 1.0);
        return baseRequirement * (1.0 - stressReduction);
    }


    /// <summary>
    ///     Determines crop growth stage based on days since sowing and growth stage durations.
    /// </summary>
    private string GetCropStage(int daysSinceSowing, CropCoefficientDefinition def)
    {
        if (daysSinceSowing <= def.LIni)
            return "Початкова стадія";

        if (daysSinceSowing <= def.LIni + def.LDev)
            return "Розвиток";

        if (daysSinceSowing <= def.LIni + def.LDev + def.LMid)
            return "Середній стадія";

        var totalGrowthDays = def.LIni + def.LDev + def.LMid + def.LLate;
        return daysSinceSowing <= totalGrowthDays ? "Пізня стадія" : "Збір врожаю";
    }

    /// <summary>
    ///     Returns irrigation action recommendation based on gross irrigation amount (mm).
    /// </summary>
    private string GetIrrigationAction(double grossIrrigation)
    {
        return grossIrrigation switch
        {
            <= 1.0 => "Не потрібно зрошення",
            <= 5.0 => "Легке зрошення рекомендовано",
            <= 15.0 => "Середнє зрошення рекомендовано",
            <= 25.0 => "Інтенсивне зрошення рекомендовано",
            _ => "Дуже інтенсивне зрошення рекомендовано"
        };
    }

    /// <summary>
    ///     Generates detailed irrigation recommendation notes based on current conditions.
    /// </summary>
    private string GenerateRecommendationNotes(
        double grossIrrigation,
        double? soilMoisture,
        WeatherData weather,
        SoilType? soilType = null,
        CropType? cropType = null)
    {
        var notes = new List<string>();

        // Irrigation need assessment
        if (grossIrrigation <= 0.0)
            notes.Add("Достатня вологість ґрунту та/або нещодавні опади.");
        else if (grossIrrigation > 20.0)
            notes.Add("Виявлено значний дефіцит води. Розгляньте можливість термінового зрошення.");

        if (soilMoisture.HasValue)
        {
            var vwc = soilMoisture.Value;

            var sw = soilType.HasValue && cropType.HasValue
                ? _soilWaterService.GetSoilParams(soilType.Value, cropType.Value)
                : _soilWaterService.GetDefaultSoilParams();

            var fc = sw.FieldCapacity;
            var pwp = sw.WiltingPoint;
            var mad = sw.AllowableDepletionFraction;

            // Total available water
            var taw = Math.Max(MinimumSoilMoistureEpsilon, fc - pwp);

            // Fraction depleted: 0 = at field capacity, 1 = at wilting point
            var fractionDepleted = Math.Clamp((fc - vwc) / taw, 0.0, 1.0);

            if (vwc <= pwp)
            {
                notes.Add($"Критично: VWC ({vwc:F3}) нижче PWP ({pwp:F3}). Імовірний водний стрес.");

                var netNeededMm = (fc - vwc) * DefaultRootDepth * 1000.0;
                var grossNeededMm = netNeededMm / DripIrrigationEfficiency;

                notes.Add($"Орієнтовно потрібно: {netNeededMm:F1} мм (net) ≈ " +
                          $"{grossNeededMm:F1} мм (при ефективності поливу={DripIrrigationEfficiency:P0}).");
            }
            else if (fractionDepleted >= mad)
            {
                notes.Add($"Вологість ґрунту низька ({vwc:F2} m³/m³). " +
                          $"Дефіцит ≈ {fractionDepleted:P0} від доступної вологи — " +
                          $"рекомендується зрошення (MAD = {mad:P0}).");
            }
            else if (fractionDepleted >= 0.5)
            {
                notes.Add($"Вологість ґрунту помірно низька ({vwc:F2} m³/m³). " +
                          $"Слід слідкувати — дефіцит ≈ {fractionDepleted:P0}.");
            }
            else
            {
                notes.Add($"Вологість ґрунту в нормі ({vwc:F2} m³/m³).");
            }
        }
        else
        {
            notes.Add("Дані про вологість ґрунту відсутні.");
        }

        // Weather-based recommendations
        if (weather.WindSpeed > HighWindSpeedThreshold)
            notes.Add("Сильний вітер. Уникайте зрошення за допомогою розпилювачів.");

        if (weather.Temperature > HighTemperatureThreshold)
            notes.Add("Висока температура. Розгляньте можливість вечірнього поливу.");

        return string.Join(" ", notes);
    }
}
