using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Crops;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Crops.Create;

public class CreateCropHandler : IRequestHandler<CreateCropCommand, Result<CropVarietyDto>>
{
    private readonly ILogger<CreateCropHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateCropHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILogger<CreateCropHandler> logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<CropVarietyDto>> Handle(CreateCropCommand request, CancellationToken cancellationToken)
    {
        var cropEntity = _mapper.Map<CropVariety>(request.CropVariety);
        if (cropEntity is null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність сорту культури";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var createdCrop = await _repositoryWrapper.CropVarietyRepository.CreateAsync(cropEntity);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            const string errorMsg = "Не вдалося створити сорт культури";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<CropVarietyDto>(createdCrop));
    }
}
