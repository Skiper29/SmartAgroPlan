using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Planning;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.CurrentRecommendation;

public record GetCurrentRecommendationQuery(int FieldId) : IRequest<Result<CurrentRecommendationDto>>;