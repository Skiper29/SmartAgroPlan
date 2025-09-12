using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Crops;

namespace SmartAgroPlan.BLL.MediatR.Crops.GetById;

public record GetCropByIdQuery(int Id) : IRequest<Result<CropVarietyDto>>;
