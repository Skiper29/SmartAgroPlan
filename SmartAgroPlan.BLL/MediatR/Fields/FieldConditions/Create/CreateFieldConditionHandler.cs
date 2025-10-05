using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.DAL.Entities.Fields;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Create;

public class CreateFieldConditionHandler : IRequestHandler<CreateFieldConditionCommand, Result<FieldConditionDto>>
{
    private readonly ILogger<CreateFieldConditionHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CreateFieldConditionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILogger<CreateFieldConditionHandler> logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<FieldConditionDto>> Handle(CreateFieldConditionCommand request,
        CancellationToken cancellationToken)
    {
        var fieldConditionEntity = _mapper.Map<FieldCondition>(request.NewFieldCondition);
        if (fieldConditionEntity == null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність стану поля";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (fieldConditionEntity.RecordedAt == default)
        {
            fieldConditionEntity.RecordedAt = DateTime.UtcNow;
        }

        var createdEntity = await _repositoryWrapper.FieldConditionRepository.CreateAsync(fieldConditionEntity);
        var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            const string errorMsg = "Не вдалося створити стан поля";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<FieldConditionDto>(createdEntity));
    }
}
