using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Crops;

namespace SmartAgroPlan.BLL.MediatR.Crops.Update;

public record UpdateCropCommand(CropVarietyUpdateDto CropVariety) : IRequest<Result<CropVarietyDto>>;
