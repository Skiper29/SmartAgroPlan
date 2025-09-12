using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Crops.GetById;

public class GetCropByIdHandler : IRequestHandler<GetCropByIdQuery, Result<CropVarietyDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetCropByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<CropVarietyDto>> Handle(GetCropByIdQuery request, CancellationToken cancellationToken)
    {
        var crop = await _repositoryWrapper.CropVarietyRepository.GetFirstOrDefaultAsync(
            c => c.Id == request.Id,
            variety => variety.Include(v => v.OptimalSoil)!);

        if (crop is null)
        {
            var errorMsg = $"Не вдалося отримати сорт культури з Id = {request.Id}";
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<CropVarietyDto>(crop));
    }
}
