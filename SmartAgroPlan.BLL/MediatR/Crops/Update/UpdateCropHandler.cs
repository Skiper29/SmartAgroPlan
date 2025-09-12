using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Crops.Update;

public class UpdateCropHandler : IRequestHandler<UpdateCropCommand, Result<CropVarietyDto>>
{
    private readonly ILogger<UpdateCropHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateCropHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILogger<UpdateCropHandler> logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<CropVarietyDto>> Handle(UpdateCropCommand request, CancellationToken cancellationToken)
    {
        var cropEntity = _mapper.Map<CropVariety>(request.CropVariety);
        if (cropEntity is null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність сорту культури";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var optimalSoil = await _repositoryWrapper.SoilRepository
            .GetFirstOrDefaultAsync(s => s.Id == request.CropVariety.OptimalSoilId);

        if (optimalSoil is null)
        {
            var errorMsg = $"Не вдалося знайти тип ґрунту з Id = {request.CropVariety.OptimalSoilId}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        cropEntity.OptimalSoil = optimalSoil;

        _repositoryWrapper.CropVarietyRepository.Update(cropEntity);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            const string errorMsg = "Не вдалося оновити сорт культури";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<CropVarietyDto>(cropEntity));
    }
}
