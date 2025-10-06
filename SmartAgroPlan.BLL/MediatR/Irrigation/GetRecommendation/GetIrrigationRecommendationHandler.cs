using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Irrigation;
using SmartAgroPlan.BLL.Interfaces.Crops;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Irrigation.GetRecommendation;

public class GetIrrigationRecommendationHandler : IRequestHandler<GetIrrigationRecommendationCommand,
    Result<IrrigationRecommendationDto>>
{
    private readonly ICropCoefficientService _cropCoefficientService;
    private readonly ILogger<GetIrrigationRecommendationHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetIrrigationRecommendationHandler(
        IRepositoryWrapper repositoryWrapper,
        ICropCoefficientService cropCoefficientService,
        ILogger<GetIrrigationRecommendationHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _cropCoefficientService = cropCoefficientService;
        _logger = logger;
    }

    public async Task<Result<IrrigationRecommendationDto>> Handle(GetIrrigationRecommendationCommand request,
        CancellationToken cancellationToken)
    {
        var field = await _repositoryWrapper.FieldRepository
            .GetFirstOrDefaultAsync(f => f.Id == request.FieldId,
                f => f
                    .Include(e => e.CurrentCrop)
                    .Include(e => e.Soil)
                    .Include(e => e.Conditions)!);

        if (field == null)
        {
            var errorMsg = $"Не вдалося знайти поле з Id = {request.FieldId}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var definition = _repositoryWrapper.CropCoefficientDefinitionRepository
            .GetFirstOrDefaultAsync(ccd => ccd.CropType == field.CurrentCrop!.CropType).Result;

        if (definition == null)
        {
            var errorMsg =
                $"Не вдалося знайти визначення коефіцієнта культури для типу культури {field.CurrentCrop!.CropType}";
            _logger.LogError(errorMsg);
            throw new ArgumentException(errorMsg);
        }

        var targetDate = request.Date ?? DateTime.UtcNow.Date;
        var kc = _cropCoefficientService.GetKc(definition, (DateTime)field.SowingDate!, targetDate);

        throw new NotImplementedException();
    }
}
