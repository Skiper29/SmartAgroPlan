using SmartAgroPlan.BLL.DTO.Recommendations;
using SmartAgroPlan.BLL.Enums;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Interfaces.Recommendations;

public interface IRecommendationService
{
    RecommendationResponseDto GenerateWeekly(Field field, CropVariety cropVariety, GrowthStage stage, DateTime currentDate);
}
