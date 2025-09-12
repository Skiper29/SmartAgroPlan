using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Crops.Delete;

public class DeleteCropHandler : IRequestHandler<DeleteCropCommand, Result<Unit>>
{
    private readonly ILogger<DeleteCropHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteCropHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILogger<DeleteCropHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteCropCommand request, CancellationToken cancellationToken)
    {
        var cropEntity =
            await _repositoryWrapper.CropVarietyRepository.GetFirstOrDefaultAsync(cv => cv.Id == request.Id);
        if (cropEntity is null)
        {
            var errorMsg = $"Не вдалося отримати сорт культури з Id = {request.Id}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repositoryWrapper.CropVarietyRepository.Delete(cropEntity);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            const string errorMsg = "Не вдалося видалити сорт культури";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(Unit.Value);
    }
}
