using MediatR;
using SmartAgroPlan.BLL.DTO.Recommendations;

namespace SmartAgroPlan.BLL.MediatR.Recommendations.Get;

public record GetRecommendationsQuery(RecommendationRequestDto RecommendationRequest)
    : IRequest<RecommendationResponseDto>;
