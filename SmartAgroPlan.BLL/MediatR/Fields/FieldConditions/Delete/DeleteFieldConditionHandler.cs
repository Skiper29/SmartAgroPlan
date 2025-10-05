using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartAgroPlan.DAL.Repositories.Repositories.Interfaces.Base;

namespace SmartAgroPlan.BLL.MediatR.Fields.FieldConditions.Delete;

public class DeleteFieldConditionHandler : IRequestHandler<DeleteFieldConditionCommand, Result<Unit>>
{
    private readonly ILogger<DeleteFieldConditionHandler> _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public DeleteFieldConditionHandler(
        IRepositoryWrapper repositoryWrapper,
        ILogger<DeleteFieldConditionHandler> logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteFieldConditionCommand request, CancellationToken cancellationToken)
    {
        var fieldConditionEntity = await _repositoryWrapper.FieldConditionRepository
            .GetFirstOrDefaultAsync(fc => fc.Id == request.Id);

        if (fieldConditionEntity is null)
        {
            var errorMsg = $"Не вдалося знайти стан поля з Id = {request.Id}";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _repositoryWrapper.FieldConditionRepository.Delete(fieldConditionEntity);
        var isSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (!isSuccess)
        {
            const string errorMsg = "Не вдалося видалити стан поля";
            _logger.LogError(errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(Unit.Value);
    }
}
