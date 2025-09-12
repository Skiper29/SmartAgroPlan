using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Crops;

namespace SmartAgroPlan.BLL.MediatR.Crops.Create;

public record CreateCropCommand(CropVarietyCreateDto CropVariety) : IRequest<Result<CropVarietyDto>>;
