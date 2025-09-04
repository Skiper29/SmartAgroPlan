using SmartAgroPlan.BLL.Enums;
using SmartAgroPlan.BLL.Interfaces.Recommendations;

namespace SmartAgroPlan.BLL.Services.Recommendations;

public class GrowthStageService : IGrowthStageService
{
    public static readonly IReadOnlyDictionary<GrowthStage, double> StageDurations = new Dictionary<GrowthStage, double>
    {
        { GrowthStage.Germination, 0.1 },
        { GrowthStage.Vegetative, 0.3 },
        { GrowthStage.Flowering, 0.20 },
        { GrowthStage.GrainFilling, 0.25 },
        { GrowthStage.Maturity, 0.15 }
    };

    private static readonly Dictionary<double, GrowthStage> _stages = new()
    {
        { 0.0, GrowthStage.Sowing },
        { 0.1, GrowthStage.Germination },
        { 0.4, GrowthStage.Vegetative },
        { 0.6, GrowthStage.Flowering },
        { 0.85, GrowthStage.GrainFilling },
        { 1.0, GrowthStage.Maturity },
        { double.MaxValue, GrowthStage.Harvest }
    };

    private const double Epsilon = 1e-6;

    public GrowthStage GetStage(DateTime plantingDate, DateTime currentDate, int growingDuration)
    {
        var growingDays = (currentDate - plantingDate).Days;
        double progress = (double)growingDays / growingDuration;

        return _stages
            .OrderBy(s => s.Key)
            .FirstOrDefault(s => progress - s.Key <= Epsilon)
            .Value;
    }
}
