using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Irrigation;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetSchedule;

public record GetWeeklyIrrigationScheduleCommand(int FieldId, DateTime? StartDate)
    : IRequest<Result<WeeklyIrrigationScheduleDto>>;
