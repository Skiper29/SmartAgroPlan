using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Analysis.GetNutrientBalance;

public record GetNutrientBalanceQuery(int FieldId) : IRequest<Result<NutrientBalanceDto>>;
