using SmartAgroPlan.BLL.Models.FertilizerForecasting;
using SmartAgroPlan.BLL.Models.Weather;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Entities.Irrigation;

namespace SmartAgroPlan.BLL.Interfaces.FertilizerForecasting;

public interface IFertilizerCalculationService
{
    public SeasonFertilizerPlan CalculateSeasonPlan(
        CropVariety crop,
        Soil soil,
        FieldCondition currentCondition,
        Field field,
        double targetYield,
        DateTime sowingDate);


    public CurrentRecommendation CalculateCurrentRecommendation(
        Field field,
        CropVariety crop,
        FieldCondition latestCondition,
        DateTime sowingDate,
        List<IrrigationRecommendation> recentIrrigation,
        List<WeatherData> weatherForecast = null!);
}
