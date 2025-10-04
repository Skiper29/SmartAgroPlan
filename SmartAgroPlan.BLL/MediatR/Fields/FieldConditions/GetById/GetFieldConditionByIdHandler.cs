using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetById;

public class GetFieldConditionByIdHandler : IRequestHandler<GetFieldConditionByIdQuery, Result<FieldConditionDto>>
{
    private readonly ILogger<GetFieldConditionByIdHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetFieldConditionByIdHandler(IMapper mapper, IRepositoryWrapper repository,
        ILogger<GetFieldConditionByIdHandler> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<FieldConditionDto>> Handle(GetFieldConditionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var fieldCondition =
            await _repository.FieldConditionRepository.GetFirstOrDefaultAsync(fc => fc.Id == request.Id);
        if (fieldCondition == null)
        {
            var errorMsg = $"Не вдалося отримати стан поля з Id = {request.Id}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<FieldConditionDto>(fieldCondition));
    }
}
