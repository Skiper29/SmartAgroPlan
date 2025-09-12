using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Crops.GetAll;

public class GetAllCropsHandler : IRequestHandler<GetAllCropsQuery, Result<IEnumerable<CropVarietyDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetAllCropsHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Result<IEnumerable<CropVarietyDto>>> Handle(GetAllCropsQuery request,
        CancellationToken cancellationToken)
    {
        var crops = await _repositoryWrapper.CropVarietyRepository.GetAllAsync(
            include: varieties =>
                varieties.Include(v => v.OptimalSoil)!);

        if (!crops.Any())
        {
            var errorMsg = "Не вдалося отримати сорти культур";
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<IEnumerable<CropVarietyDto>>(crops));
    }
}
