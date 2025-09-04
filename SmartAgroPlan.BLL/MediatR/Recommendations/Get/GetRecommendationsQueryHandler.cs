using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.DTO.Recommendations;
using SmartAgroPlan.BLL.Interfaces.Recommendations;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Recommendations.Get;

public class GetRecommendationsQueryHandler : IRequestHandler<GetRecommendationsQuery, RecommendationResponseDto>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGrowthStageService _growthStageService;
    private readonly IRecommendationService _recommendationService;

    public GetRecommendationsQueryHandler(
        IRepositoryWrapper repositoryWrapper,
        IGrowthStageService growthStageService,
        IRecommendationService recommendationService)
    {
        _repositoryWrapper = repositoryWrapper;
        _growthStageService = growthStageService;
        _recommendationService = recommendationService;
    }

    public async Task<RecommendationResponseDto> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var field = await _repositoryWrapper.FieldRepository.FindAll(
            predicate: f => f.Id == request.RecommendationRequest.FieldId,
            include: f => f
                .Include(field => field.CurrentCrop)!)
            .FirstOrDefaultAsync();

        if (field == null)
        {
            throw new KeyNotFoundException($"Field with id {request.RecommendationRequest.FieldId} not found");
        }

        if (field.CurrentCrop == null)
        {
            throw new InvalidOperationException($"Field with id {request.RecommendationRequest.FieldId} has no current crop");
        }

        var currentGrowthStage = _growthStageService.GetStage((DateTime)field.SowingDate!, request.RecommendationRequest.CurrentDate, field.CurrentCrop.GrowingDuration);

        return _recommendationService.GenerateWeekly(field, field.CurrentCrop, currentGrowthStage, request.RecommendationRequest.CurrentDate);
    }
}
