using FluentResults;
using MediatR;

namespace SmartAgroPlan.BLL.MediatR.Crops.Delete;

public record DeleteCropCommand(int Id) : IRequest<Result<Unit>>;
