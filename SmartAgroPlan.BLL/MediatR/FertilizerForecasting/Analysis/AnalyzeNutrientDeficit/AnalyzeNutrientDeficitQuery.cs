using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.FertilizerForecasting.Analysis;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Analysis.AnalyzeNutrientDeficit;

public record AnalyzeNutrientDeficitQuery(int FieldId) : IRequest<Result<NutrientDeficitAnalysisDto>>;
