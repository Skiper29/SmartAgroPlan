using FluentResults;
using MediatR;

namespace SmartAgroPlan.BLL.MediatR.FertilizerForecasting.Applications.RecordApplication;

public record RecordApplicationCommand(
    int FieldId,
    DateTime ApplicationDate,
    int ApplicationMethodId,
    Dictionary<int, double> ProductsUsed,
    int? ApplicationPlanId,
    string? Notes,
    double? Temperature,
    double? WindSpeed,
    double? Humidity
) : IRequest<Result<int>>;