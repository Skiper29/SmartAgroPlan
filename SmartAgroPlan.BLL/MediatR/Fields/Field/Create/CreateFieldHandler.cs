using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.Field.Create;

public class CreateFieldHandler : IRequestHandler<CreateFieldCommand, Result<FieldDto>>
{
    private readonly ILogger<CreateFieldHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateFieldHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILogger<CreateFieldHandler> logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<FieldDto>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        var fieldEntity = _mapper.Map<DAL.Entities.Fields.Field>(request.NewField);
        if (fieldEntity == null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність поля";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        // Set timestamps
        fieldEntity.CreatedAt = DateTime.UtcNow;
        fieldEntity.UpdatedAt = DateTime.UtcNow;

        var createdEntity = await _repositoryWrapper.FieldRepository.CreateAsync(fieldEntity);
        var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            const string errorMsg = "Не вдалося створити поле";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        // If a crop is assigned and sowing date is provided, create a crop history entry
        if (createdEntity.CurrentCropId.HasValue && createdEntity.SowingDate.HasValue)
        {
            var cropHistory = new FieldCropHistory
            {
                FieldId = createdEntity.Id,
                CropId = createdEntity.CurrentCropId.Value,
                PlantedDate = DateOnly.FromDateTime(createdEntity.SowingDate.Value),
                Notes = "Початковий запис при створенні поля"
            };

            await _repositoryWrapper.FieldCropHistoryRepository.CreateAsync(cropHistory);
            await _repositoryWrapper.SaveChangesAsync();
        }

        return Result.Ok(_mapper.Map<FieldDto>(createdEntity));
    }
}
