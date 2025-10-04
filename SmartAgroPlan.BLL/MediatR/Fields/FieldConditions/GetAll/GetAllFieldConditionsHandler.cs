using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.FieldConditions;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.GetAll;

public class
    GetAllFieldConditionsHandler : IRequestHandler<GetAllFieldConditionsQuery, Result<IEnumerable<FieldConditionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;

    public GetAllFieldConditionsHandler(IMapper mapper, IRepositoryWrapper repository,
        ILogger<GetAllFieldConditionsHandler> logger)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<IEnumerable<FieldConditionDto>>> Handle(GetAllFieldConditionsQuery request,
        CancellationToken cancellationToken)
    {
        var fieldConditions =
            await _repository.FieldConditionRepository.GetAllAsync();

        var fieldConditionsDto = _mapper.Map<IEnumerable<FieldConditionDto>>(fieldConditions);

        return Result.Ok(fieldConditionsDto);
    }
}
