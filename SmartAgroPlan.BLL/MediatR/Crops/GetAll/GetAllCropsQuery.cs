using FluentResults;
using MediatR;
using SmartAgroPlan.BLL.DTO.Crops;

namespace SmartAgroPlan.BLL.MediatR.Crops.GetAll;

public record GetAllCropsQuery : IRequest<Result<IEnumerable<CropVarietyDto>>>;
