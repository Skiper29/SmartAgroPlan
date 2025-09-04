using SmartAgroPlan.BLL.Enums;

namespace SmartAgroPlan.BLL.Interfaces.Recommendations;

public interface IGrowthStageService
{
    GrowthStage GetStage(DateTime plantingDate, DateTime currentDate, int growingDuration);
}
