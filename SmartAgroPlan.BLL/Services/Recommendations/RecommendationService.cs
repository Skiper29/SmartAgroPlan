using SmartAgroPlan.BLL.DTO.Recommendations;
using SmartAgroPlan.BLL.Enums;
using SmartAgroPlan.BLL.Interfaces.Recommendations;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Services.Recommendations;

public class RecommendationService : IRecommendationService
{
    private readonly Dictionary<GrowthStage, double> waterDistribution = new()
    {
        { GrowthStage.Germination, 0.1 },
        { GrowthStage.Vegetative, 0.3 },
        { GrowthStage.Flowering, 0.25 },
        { GrowthStage.GrainFilling, 0.25 },
        { GrowthStage.Maturity, 0.1 }
    };

    private readonly Dictionary<GrowthStage, double> fertDistribution = new()
    {
        { GrowthStage.Germination, 0.2 },
        { GrowthStage.Vegetative, 0.4 },
        { GrowthStage.Flowering, 0.3 },
        { GrowthStage.GrainFilling, 0.1 }
    };

    public RecommendationResponseDto GenerateWeekly(Field field, CropVariety cropVariety, GrowthStage stage, DateTime currentDate)
    {
        var totalWater = cropVariety.WaterRequirement;
        var totalFertilizer = cropVariety.FertilizerRequirement;

        if (!GrowthStageService.StageDurations.ContainsKey(stage))
        {
            return new RecommendationResponseDto
            {
                FieldId = field.Id,
                Crop = cropVariety.Name ?? "Unknown",
                GrowthStage = stage.ToString(),
                WaterRecommendationMm = 0,
                FertilizerRecommendationKgHa = 0,
                Notes = "No recommendations for this stage."
            };
        }

        int stageDurationDays = (int)Math.Round(cropVariety.GrowingDuration * GrowthStageService.StageDurations[stage]);

        double waterForStage = totalWater * waterDistribution.GetValueOrDefault(stage, 0);
        double fertForStage = totalFertilizer * fertDistribution.GetValueOrDefault(stage, 0);

        int stageWeeks = Math.Max(1, stageDurationDays / 7);
        double weeklyWater = waterForStage / stageWeeks;
        double weeklyFert = fertForStage / stageWeeks;

        return new RecommendationResponseDto
        {
            FieldId = field.Id,
            Crop = cropVariety.Name ?? "Unknown",
            GrowthStage = stage.ToString(),
            WaterRecommendationMm = Math.Round(weeklyWater, 2),
            FertilizerRecommendationKgHa = Math.Round(weeklyFert, 2),
            Notes = $"Weekly recommendation for {stage}. " +
                    $"Stage lasts ~{stageDurationDays} days. Adjust for soil and weather."
        };
    }
}
