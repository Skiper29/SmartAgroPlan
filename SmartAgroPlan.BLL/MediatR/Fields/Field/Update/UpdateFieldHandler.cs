using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.BLL.DTO.Fields.Field;
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
        var fieldEntity = _mapper.Map<DAL.Entities.Fields.Field>(request.UpdatedField);
        if (fieldEntity == null)
        {
            const string errorMsg = "Не вдалося відобразити DTO в сутність поля";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repositoryWrapper.FieldRepository.Update(fieldEntity);
        var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            const string errorMsg = "Не вдалося оновити поле";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<FieldDto>(fieldEntity));
    }
}
