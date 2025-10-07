using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Irrigation;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;

public record GetIrrigationRecommendationCommand : IRequest<Result<IrrigationRecommendationDto>>
{
    public int FieldId { get; set; }
    public DateTime? Date { get; set; } // Optional, defaults to today
    public bool IncludeForecast { get; set; } = false;
    public int ForecastDays { get; set; } = 7;
}
