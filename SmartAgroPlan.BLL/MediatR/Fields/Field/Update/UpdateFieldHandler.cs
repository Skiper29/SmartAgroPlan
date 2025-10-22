using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.Update;

public class UpdateFieldHandler : IRequestHandler<UpdateFieldCommand, Result<FieldDto>>
{
    private readonly ILogger<UpdateFieldHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public UpdateFieldHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILogger<UpdateFieldHandler> logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<FieldDto>> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        // Get existing field to check for crop changes
        var existingField =
            await _repositoryWrapper.FieldRepository.GetFirstOrDefaultAsync(f => f.Id == request.UpdatedField.Id);
        if (existingField == null)
        {
            const string errorMsg = "Поле не знайдено";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var fieldEntity = _mapper.Map<DAL.Entities.Fields.Field>(request.UpdatedField);
        if (fieldEntity == null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність поля";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        // Update timestamp
        fieldEntity.UpdatedAt = DateTime.UtcNow;
        fieldEntity.CreatedAt = existingField.CreatedAt; // Preserve original creation date

        // Check if crop has changed
        var cropChanged = existingField.CurrentCropId != fieldEntity.CurrentCropId;

        _repositoryWrapper.FieldRepository.Update(fieldEntity);
        var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            const string errorMsg = "Не вдалося оновити поле";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        // If crop changed and new crop is assigned with sowing date, create crop history entry
        if (cropChanged && fieldEntity.CurrentCropId.HasValue && fieldEntity.SowingDate.HasValue)
        {
            // Close previous crop history if exists
            if (existingField.CurrentCropId.HasValue)
            {
                var previousHistory = await _repositoryWrapper.FieldCropHistoryRepository
                    .GetAllAsync(h => h.FieldId == fieldEntity.Id &&
                                      h.CropId == existingField.CurrentCropId.Value &&
                                      !h.HarvestedDate.HasValue);

                var previousHistoryList = previousHistory.ToList();
                if (previousHistoryList.Any())
                {
                    var lastHistory = previousHistoryList.OrderByDescending(h => h.PlantedDate).First();
                    lastHistory.HarvestedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    lastHistory.Notes +=
                        " | Закрито через зміну посіву | Додайте урожайність будь ласка і змініть дату збору врожаю якщо потрібно.";
                    _repositoryWrapper.FieldCropHistoryRepository.Update(lastHistory);
                }
            }

            // Create new crop history entry
            var cropHistory = new FieldCropHistory
            {
                FieldId = fieldEntity.Id,
                CropId = fieldEntity.CurrentCropId.Value,
                PlantedDate = DateOnly.FromDateTime(fieldEntity.SowingDate.Value),
                Notes = "Культура оновлена через зміну посіву"
            };

            await _repositoryWrapper.FieldCropHistoryRepository.CreateAsync(cropHistory);
            await _repositoryWrapper.SaveChangesAsync();
        }

        return Result.Ok(_mapper.Map<FieldDto>(fieldEntity));
    }
}
