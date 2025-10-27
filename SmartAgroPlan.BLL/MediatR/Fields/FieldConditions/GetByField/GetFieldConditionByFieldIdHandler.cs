using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetByField;

public class
    GetFieldConditionByFieldIdHandler : IRequestHandler<GetFieldConditionByFieldIdQuery,
    Result<List<FieldConditionDto>>>
{
    private readonly ILogger<GetFieldConditionByFieldIdHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public GetFieldConditionByFieldIdHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        ILogger<GetFieldConditionByFieldIdHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<FieldConditionDto>>> Handle(
        GetFieldConditionByFieldIdQuery request,
        CancellationToken cancellationToken)
    {
        var fieldConditions = await _repositoryWrapper.FieldConditionRepository
            .GetAllAsync(fc => fc.FieldId == request.FieldId);

        if (!fieldConditions.Any())
        {
            var errorMsg = $"Умови поля з ID {request.FieldId} не знайдено.";
            _logger.LogWarning(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<List<FieldConditionDto>>(fieldConditions));
    }
}
