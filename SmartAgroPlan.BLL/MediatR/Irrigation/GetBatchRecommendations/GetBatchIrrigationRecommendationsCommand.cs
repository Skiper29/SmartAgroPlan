using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Irrigation;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetBatchRecommendations;

public record GetBatchIrrigationRecommendationsCommand(List<int> FieldIds, DateTime? Date)
    : IRequest<Result<List<IrrigationRecommendationDto>>>;
